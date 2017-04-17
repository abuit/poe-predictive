using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace POEStash.Model.Converters
{
    class PropertyValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(JsonPOEPropertyValue));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            if (array.Count == 0)
            {
                return new JsonPOEPropertyValue
                {
                    Value = string.Empty,
                    ValueType = 0
                };
            }

            return new JsonPOEPropertyValue
            {
                Value = (string)array[0][0],
                ValueType = (ValueType)(int)array[0][1]
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}