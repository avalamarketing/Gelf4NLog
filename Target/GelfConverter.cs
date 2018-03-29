using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using Newtonsoft.Json.Linq;
using Gelf4NLog.Target.Extensions;

namespace Gelf4NLog.Target
{
    public class GelfConverter : IConverter
    {
        private const int ShortMessageMaxLength = 250;
        private const string GelfVersion = "1.1";

        public JObject GetGelfJson(LogEventInfo logEventInfo, string application, string environment, IList<RedactInfo> redactions)
        {
            //Retrieve the formatted message from LogEventInfo
            var logEventMessage = redactions.Aggregate(logEventInfo.FormattedMessage,
                (current, redaction) => redaction.LazyRegex.Value.Replace(current, redaction.Replacement));

            if (logEventMessage == null) return null;

            //Construct the instance of GelfMessage
            //See http://docs.graylog.org/en/2.1/pages/gelf.html?highlight=short%20message#gelf-format-specification "Specification (version 1.1)"
            var gelfMessage = new GelfMessage
            {
                Version = GelfVersion,
                Host = Dns.GetHostName().ToUpper(),
                ShortMessage = GetShortMessage(logEventMessage),
                FullMessage = logEventMessage,
                Timestamp = logEventInfo.TimeStamp.ToUnixTimestamp(),
                Level = GetSeverityLevel(logEventInfo.Level)
            };

            //Convert to JSON
            var jsonObject = JObject.FromObject(gelfMessage);

            //Add any other interesting data to additional fields
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("application", application));
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("environment", environment));
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("line", logEventInfo.UserStackFrame?.GetFileLineNumber().ToString(CultureInfo.InvariantCulture)));
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("file", logEventInfo.UserStackFrame?.GetFileName()));
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("LoggerName", logEventInfo.LoggerName));
            AddAdditionalField(jsonObject, new KeyValuePair<object, object>("LogLevelName", logEventInfo.Level?.ToString()));

            //If we are dealing with an exception, add exception properties as additional fields
            if (logEventInfo.Exception != null)
            {
                string exceptionDetail;
                string stackDetail;

                GetExceptionMessages(logEventInfo.Exception, out exceptionDetail, out stackDetail);
                
                AddAdditionalField(jsonObject, new KeyValuePair<object, object>("ExceptionSource", logEventInfo.Exception.Source));
                AddAdditionalField(jsonObject, new KeyValuePair<object, object>("ExceptionMessage", exceptionDetail));
                AddAdditionalField(jsonObject, new KeyValuePair<object, object>("StackTrace", stackDetail));
            }

            foreach (var property in logEventInfo.Properties)
            {
                AddAdditionalField(jsonObject, property);
            }

            return jsonObject;
        }

        private static string GetShortMessage(string logEventMessage)
        {
            //Figure out the short message
            var shortMessage = logEventMessage;
            if (shortMessage.Length > ShortMessageMaxLength)
            {
                shortMessage = shortMessage.Substring(0, ShortMessageMaxLength);
            }
            return shortMessage;
        }

        /// <summary>
        /// Values from SyslogSeverity enum here: http://marc.info/?l=log4net-dev&amp;m=109519564630799
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int GetSeverityLevel(LogLevel level)
        {
            if (level == LogLevel.Debug)
            {
                return 7;
            }
            if (level == LogLevel.Fatal)
            {
                return 2;
            }
            if (level == LogLevel.Info)
            {
                return 6;
            }
            if (level == LogLevel.Trace)
            {
                return 6;
            }
            if (level == LogLevel.Warn)
            {
                return 4;
            }

            return 3; //LogLevel.Error
        }

        


        /// <summary>
        /// Get the message details from all nested exceptions, up to 10 in depth.
        /// </summary>
        /// <param name="ex">Exception to get details for</param>
        /// <param name="exceptionDetail">Exception message</param>
        /// <param name="stackDetail">Stacktrace with inner exceptions</param>
        private void GetExceptionMessages(Exception ex, out string exceptionDetail, out string stackDetail)
        {
            var exceptionSb = new StringBuilder();
            var stackSb = new StringBuilder();
            var nestedException = ex;
            stackDetail = null;

            var counter = 0;
            do
            {
                exceptionSb.Append($"{nestedException.Message} - ");
                if (nestedException.StackTrace != null)
                    stackSb.Append($"{nestedException.StackTrace}{Environment.NewLine}--- Inner exception stack trace ---{Environment.NewLine}");
                nestedException = nestedException.InnerException;
                counter++;
            }
            while (nestedException != null && counter < 11);

            exceptionDetail = exceptionSb.ToString().Substring(0, exceptionSb.Length - 3);
            if (stackSb.Length > 0)
                stackDetail = stackSb.ToString().Substring(0, stackSb.Length - 35);
        }

        private static void AddAdditionalField(IDictionary<string, JToken> jObject, KeyValuePair<object, object> property)
        {
            var key = property.Key as string;
            var value = property.Value as string;

            if (key == null || value == null) return;

            //According to the GELF spec, libraries should NOT allow to send id as additional field (_id)
            //Server MUST skip the field because it could override the MongoDB _key field
            if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                key = "id_";

            //According to the GELF spec, additional field keys should start with '_' to avoid collision
            if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                key = "_" + key;

            jObject.Add(key, value);
        }
    }
}
