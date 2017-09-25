using Gelf4NLog.Target;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gelf4NLog.UnitTest
{
    public class NLogTargetIntegrationTests
    {
        [Fact]
        public void SendMessageWithFieldLongerThan32766Bytes()
        {            
            var logEventWithFieldLongerThan32766Bytes = new LogEventInfo();
            logEventWithFieldLongerThan32766Bytes.Message = "Testing defect axod-9757.";
            logEventWithFieldLongerThan32766Bytes.Properties.Add("Tennant", new string('a', 32766 +1).Replace("a", "▨👊👍"));
            logEventWithFieldLongerThan32766Bytes.Properties.Add("CurrentUser", new string('a', 32766 + 1).Replace("a", "a👊"));

            var sut = new NLogTarget();
            sut.Endpoint = "udp://logs.aimbase.net:12201";
            sut.Application = "Gelf4Nlog Integration Tests";
            sut.Environment = "DEV";

            sut.WriteLogEventInfo(logEventWithFieldLongerThan32766Bytes);
        }
    }
}
