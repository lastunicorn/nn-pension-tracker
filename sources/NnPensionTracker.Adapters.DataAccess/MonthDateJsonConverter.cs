using System.Text.Json;
using System.Text.Json.Serialization;
using DustInTheWind.NN.Toolkit.MandatoryPrivatePension;

namespace DustInTheWind.NnPensionTracker.Adapters.DataAccess;

public class MonthDateJsonConverter : JsonConverter<MonthDate>
{
    public override MonthDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString();
            return MonthDate.Parse(value);
        }

        // Fallback: handle legacy format {"Year": 2025, "Month": 5}
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            int year = 0, month = 0;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propName = reader.GetString();
                    reader.Read();
                    if (propName == "Year") year = reader.GetInt32();
                    else if (propName == "Month") month = reader.GetInt32();
                }
            }
            return new MonthDate(year, month);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when reading MonthDate.");
    }

    public override void Write(Utf8JsonWriter writer, MonthDate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

