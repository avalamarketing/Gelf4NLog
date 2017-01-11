using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NLog.Config;

namespace Gelf4NLog.Target
{
    [NLogConfigurationItem]
    public class RedactInfo
    {
        [RequiredParameter]
        public string Pattern { get; set; }

        [RequiredParameter]
        [DefaultValue("REDACTED")]
        public string Replacement { get; set; }

        public Lazy<Regex> LazyRegex { get; }

        public RedactInfo()
        {
            LazyRegex = new Lazy<Regex>(() =>
            {
                var regex = new Regex(Pattern, RegexOptions.Compiled);
                return regex;
            });
        }
    }
}