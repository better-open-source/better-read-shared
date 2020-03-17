using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class CseThumbnail
    {
        [JsonProperty("src")]
        public Uri Src { get; set; }

        [JsonProperty("width")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Width { get; set; }

        [JsonProperty("height")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Height { get; set; }
    }
}