using System;
using System.Collections.Generic;
using System.Net;
using Gelf4NLog.Target;
using Gelf4NLog.Target.Extensions;
using NLog;
using Xunit;

namespace Gelf4NLog.UnitTest
{
    public class GelfConverterTests
    {
        [Fact]
        public void ShouldCreateGelfJsonCorrectly()
        {
            var timestamp = DateTime.Now;
            var logEvent = new LogEventInfo
            {
                Message = "Test Log Message",
                Level = LogLevel.Info,
                TimeStamp = timestamp,
                LoggerName = "GelfConverterTestLogger"
            };
            logEvent.Properties.Add("customproperty1", "customvalue1");
            logEvent.Properties.Add("customproperty2", "customvalue2");

            var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility", "DEV", new List<RedactInfo>());

            Assert.NotNull(jsonObject);
            Assert.Equal("1.1", jsonObject.Value<string>("version"));
            Assert.Equal(Dns.GetHostName().ToUpper(), jsonObject.Value<string>("host"));
            Assert.Equal("Test Log Message", jsonObject.Value<string>("short_message"));
            Assert.Equal("Test Log Message", jsonObject.Value<string>("full_message"));
            Assert.Equal(timestamp.ToUnixTimestamp(), jsonObject.Value<Decimal>("timestamp"));
            Assert.Equal(6, jsonObject.Value<int>("level"));
            Assert.Equal("TestFacility", jsonObject.Value<string>("_application"));
            Assert.Equal("DEV", jsonObject.Value<string>("_environment"));

            Assert.Equal("customvalue1", jsonObject.Value<string>("_customproperty1"));
            Assert.Equal("customvalue2", jsonObject.Value<string>("_customproperty2"));
            Assert.Equal("GelfConverterTestLogger", jsonObject.Value<string>("_LoggerName"));
            Assert.Equal("Info", jsonObject.Value<string>("_LogLevelName"));

            //make sure that there are no other junk in there
            Assert.Equal(12, jsonObject.Count);
        }

        [Fact]
        public void ShouldHandleExceptionsCorrectly()
        {
            var logEvent = new LogEventInfo
            {
                Message = "Test Message",
                Exception = new DivideByZeroException("div by 0")
            };

            var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility", "DEV", new List<RedactInfo>());

            Assert.NotNull(jsonObject);
            Assert.Equal("Test Message", jsonObject.Value<string>("short_message"));
            Assert.Equal("Test Message", jsonObject.Value<string>("full_message"));
            Assert.Equal(3, jsonObject.Value<int>("level"));
            Assert.Equal("TestFacility", jsonObject.Value<string>("_application"));
            Assert.Equal("DEV", jsonObject.Value<string>("_environment"));
            Assert.Equal(null, jsonObject.Value<string>("_ExceptionSource"));
            Assert.Equal("div by 0", jsonObject.Value<string>("_ExceptionMessage"));
            Assert.Equal(null, jsonObject.Value<string>("_StackTrace"));
            Assert.Equal(null, jsonObject.Value<string>("_LoggerName"));
        }

        [Fact]
        public void ShouldHandleLongMessageCorrectly()
        {
            var logEvent = new LogEventInfo
            {
                //The first 300 chars of lorem ipsum...
                Message =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus interdum est in est cursus vitae pellentesque felis lobortis. Donec a orci quis ante viverra eleifend ac et quam. Donec imperdiet libero ut justo tincidunt non tristique mauris gravida. Fusce sapien eros, tincidunt a placerat nullam."
            };

            var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility", "DEV", new List<RedactInfo>());

            Assert.NotNull(jsonObject);
            Assert.Equal(250, jsonObject.Value<string>("short_message").Length);
            Assert.Equal(300, jsonObject.Value<string>("full_message").Length);
        }

        [Fact]
        public void ShouldHandlePropertyCalledIdProperly()
        {
            var logEvent = new LogEventInfo {Message = "Test"};
            logEvent.Properties.Add("Id", "not_important");

            var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility", "DEV", new List<RedactInfo>());

            Assert.NotNull(jsonObject);
            Assert.Null(jsonObject["_id"]);
            Assert.Equal("not_important", jsonObject.Value<string>("_id_"));
        }

        [Fact]
        public void ShouldRedactCreditCardNumber()
        {
            var logEvent = new LogEventInfo { Message = "Test 4111111111111111 Test" };
            logEvent.Properties.Add("Id", "not_important");

            var redactInfos = new List<RedactInfo>
            {
                new RedactInfo
                {
                    Pattern = "4[0-9]{12}(?:[0-9]{3})?",
                    Replacement = "REDACTED"
                }
            };

            var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility", "DEV", redactInfos);

            Assert.Equal("Test REDACTED Test", jsonObject.Value<string>("short_message"));
            Assert.Equal("Test REDACTED Test", jsonObject.Value<string>("full_message"));
        }
    }
}