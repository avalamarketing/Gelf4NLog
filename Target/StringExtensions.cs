using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gelf4NLog.Target
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int sizeInBytes)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (sizeInBytes < 0) throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Size must be greater than zero.");

            if (Encoding.UTF8.GetByteCount(value) <= sizeInBytes) return value;

            var bytes = Encoding.UTF8.GetBytes(value);

            var index = sizeInBytes - 1;

            // If the byte we're splitting on is part of a multi-byte character
            // it's first two bytes will be 10.
            if (index >= 0 && (bytes[index] & 0b1000_0000) == 0b1000_0000)
            {
                // Count back until we find a byte that starts with 11. It's the first
                // byte of a multi-byte character.
                while (index >= 0 && (bytes[index] & 0b1100_0000) != 0b1100_0000) index--;

                sizeInBytes = index;
            }

            return Encoding.UTF8.GetString(bytes, 0, sizeInBytes);
        }

        public static string Truncate(this string value, int sizeInBytes, string suffix)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (Encoding.UTF8.GetByteCount(value) > sizeInBytes)
            {
                sizeInBytes -= Encoding.UTF8.GetByteCount(suffix);
                if (sizeInBytes >= 0)
                {
                    return string.Concat(value.Truncate(sizeInBytes), suffix);
                }
            }

            return value.Truncate(sizeInBytes);
        }
    }
}
