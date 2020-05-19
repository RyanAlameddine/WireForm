using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wireform.Circuitry.Data.Bits
{
    class BitArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);


            BitValue[] items = token.ToObject<BitValue[]>();
            return new BitArray(items);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var val in ((BitArray)value).BitValues)
            {
                writer.WriteValue(val.Selected);
            }
            writer.WriteEndArray();
        }
    }
}
