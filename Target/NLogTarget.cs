using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NLog;
using NLog.Targets;
using Newtonsoft.Json;
using NLog.Config;

namespace Gelf4NLog.Target
{
    [Target("GrayLog")]
    public class NLogTarget : TargetWithLayout
    {
        private readonly Lazy<IPEndPoint> lazyIpEndoint;
        private Uri endpoint;

        [RequiredParameter]
        public string Endpoint
        {
            get { return endpoint.ToString(); }
            set { endpoint = value != null ? new Uri(System.Environment.ExpandEnvironmentVariables(value)) : null; }
        }

        public string Application { get; set; }

        public string Environment { get; set; }

        [ArrayParameter(typeof(RedactInfo), "redact")]
        public IList<RedactInfo> Redactions { get; set; }

        public IConverter Converter { get; private set; }
        public ITransport Transport { get; private set; }

        public NLogTarget() : this(new UdpTransport(new UdpSocketTransportClient()), new GelfConverter())
        {
        }

        public NLogTarget(ITransport transport, IConverter converter)
        {
            Redactions = new List<RedactInfo>();
            Transport = transport;
            Converter = converter;
            lazyIpEndoint = new Lazy<IPEndPoint>(() =>
            {
                var addresses = Dns.GetHostAddresses(endpoint.Host);
                var ip = addresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);

                return new IPEndPoint(ip, endpoint.Port);
            });
        }

        public void WriteLogEventInfo(LogEventInfo logEvent)
        {
            Write(logEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var jsonObject = Converter.GetGelfJson(logEvent, Application, Environment, Redactions);
            if (jsonObject == null) return;
            Transport.Send(lazyIpEndoint.Value, jsonObject.ToString(Formatting.None, new GelfJsonStringConverter()));
        }
    }
}
