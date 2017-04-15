using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace POEStash
{
    [JsonObject]
    public class Property
    {
        [JsonProperty("name")]
        public string Name      { get; set; }
        [JsonProperty("displayMode")]
        public int DisplayMode  { get; set; }
        [JsonProperty("type")]
        public int Type         { get; set; }
        [JsonProperty("values"), JsonConverter(typeof(PropertyValueConverter))]
        public PropertyValue Values { get; set; }
        [JsonProperty("progress"), JsonConverter(typeof(JsonScientificConverter))]
        public int Progress     { get; set; }
    }

    public class PropertyValue
    {
        public string Value { get; set; }
        public ValueType ValueType { get; set; }
    }

    class PropertyValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(PropertyValue));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            if (array.Count == 0)
            {
                return new PropertyValue
                {
                    Value = string.Empty,
                    ValueType = 0
                };
            }

            return new PropertyValue
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

