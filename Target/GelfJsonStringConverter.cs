using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gelf4NLog.Target
{
    public class GelfJsonStringConverter : JsonConverter
    {
        private const int GelfMaxFieldLength = 32766;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(string));
        }

        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stringValue = value as string;

            writer.WriteValue(stringValue.Truncate(GelfMaxFieldLength, "..."));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
