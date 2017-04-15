using System;
using Newtonsoft.Json;

public class JsonScientificConverter : JsonConverter
{
    public override bool CanRead { get { return true; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return (int)(serializer.Deserialize<System.Int64>(reader));
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}
