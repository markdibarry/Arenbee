using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameCore.Statistics;

namespace GameCore.Utility.JsonConverters;

public class StatsNotifierConverter : JsonConverter<StatsNotifier>
{
    enum TypeDiscriminator
    {
        TimedNotifier = 1
    }

    public override bool CanConvert(Type typeToConvert) =>
        typeof(StatsNotifier).IsAssignableFrom(typeToConvert);

    public override StatsNotifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException();

        string propertyName = reader.GetString();
        if (propertyName != "TypeDiscriminator")
            throw new JsonException();

        reader.Read();
        if (reader.TokenType != JsonTokenType.Number)
            throw new JsonException();

        var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
        StatsNotifier statsNotifier = typeDiscriminator switch
        {
            TypeDiscriminator.TimedNotifier => new TimedNotifier(),
            _ => throw new JsonException()
        };

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return statsNotifier;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "TimeRemaining":
                        ((TimedNotifier)statsNotifier).TimeRemaining = reader.GetSingle();
                        break;
                    case "OneShot":
                        ((TimedNotifier)statsNotifier).OneShot = reader.GetBoolean();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer, StatsNotifier statsNotifier, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (statsNotifier is TimedNotifier timedNotifier)
        {
            writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.TimedNotifier);
            writer.WriteNumber("TimeRemaining", timedNotifier.TimeRemaining);
            writer.WriteBoolean("OneShot", timedNotifier.OneShot);
        }

        writer.WriteEndObject();
    }
}
