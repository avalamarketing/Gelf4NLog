using System;
using Newtonsoft.Json;

namespace Gelf4NLog.Target
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GelfMessage
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("short_message")]
        public string ShortMessage { get; set; }

        [JsonProperty("full_message")]
        public string FullMessage { get; set; }

        [JsonProperty("timestamp")]
        public decimal Timestamp { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
