using System.Collections.Generic;
using System.Net;
using Gelf4NLog.Target;
using Gelf4NLog.UnitTest.Resources;
using Moq;
using Newtonsoft.Json.Linq;
using NLog;
using Xunit;

namespace Gelf4NLog.UnitTest
{
    public class UdpTransportTests
    {
        [Fact]
        public void ShouldSendLongUdpMessage()
        {
            var jsonObject = new JObject();
            var message = ResourceHelper.GetResource("LongMessage.txt").ReadToEnd();

            jsonObject.Add("full_message", JToken.FromObject(message));

            var converter = new Mock<IConverter>();
            converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<RedactInfo>>()))
                .Returns(jsonObject)
                .Verifiable();
            var transportClient = new Mock<ITransportClient>();
            transportClient.Setup(t => t.Send(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>())).Verifiable();

            var transport = new UdpTransport(transportClient.Object);

            var target = new NLogTarget(transport, converter.Object) {Endpoint = "udp://localhost:12201"};
            target.WriteLogEventInfo(new LogEventInfo());

            transportClient.Verify(t => t.Send(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()),
                Times.Exactly(2));
            converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<RedactInfo>>()),
                Times.Once());
        }

        [Fact]
        public void ShouldSendShortUdpMessage()
        {
            var transportClient = new Mock<ITransportClient>();
            var transport = new UdpTransport(transportClient.Object);
            var converter = new Mock<IConverter>();
            converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<RedactInfo>>()))
                .Returns(new JObject());

            var target = new NLogTarget(transport, converter.Object) {Endpoint = "udp://localhost:12201"};
            var logEventInfo = new LogEventInfo {Message = "Test Message"};

            target.WriteLogEventInfo(logEventInfo);

            transportClient.Verify(t => t.Send(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<IPEndPoint>()),
                Times.Once());
            converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<RedactInfo>>()),
                Times.Once());
        }
    }
}