using Gelf4NLog.Target.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gelf4NLog.UnitTest
{
    public class DateTimeExtensionTests
    {
        [Fact]
        public void ToUnixTimestamp_TimeInUtc_ConvertsToUtcTimestamp()
        {
            DateTime cst = new DateTime(1977, 3, 4, 9, 20, 0, DateTimeKind.Utc);

            var result = cst.ToUnixTimestamp();

            Assert.Equal(226315200, result);
        }

        [Fact]
        public void ToUnixTimestamp_FirstStepsOnTheMoon_ReturnsNegativeTimestamp()
        {
            DateTime firstStepsOnTheMoon = new DateTime(1969, 7, 21, 2, 56, 15, DateTimeKind.Utc);

            var result = firstStepsOnTheMoon.ToUnixTimestamp();

            Assert.Equal(-14159025, result);
        }
    }
}
