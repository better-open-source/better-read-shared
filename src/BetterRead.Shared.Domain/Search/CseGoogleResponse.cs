using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace BetterRead.Shared.Domain.Search
{
    public partial class CseGoogleResponse
    {
        [JsonProperty("cursor")]
        public Cursor Cursor { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("results")]
        public List<Result> SearchResults { get; set; }

        [JsonProperty("findMoreOnGoogle")]
        public FindMoreOnGoogle FindMoreOnGoogle { get; set; }
    }

    public partial class CseGoogleResponse
    {
        public static CseGoogleResponse FromJson(string json) => JsonConvert.DeserializeObject<CseGoogleResponse>(json, Converter.Settings);
    }
    
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (long.TryParse(value, out var l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
