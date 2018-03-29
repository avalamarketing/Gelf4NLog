using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gelf4NLog.Target.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static decimal ToUnixTimestamp(this DateTime value)
        {
            return Convert.ToDecimal(value.ToUniversalTime().Subtract(UnixEpoch).TotalSeconds);
        }
    }
}
