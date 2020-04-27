using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Infrastructure.Domain.Api.GoogleSearch
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