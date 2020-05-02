using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Api.GoogleSearch
{
    public class Page
    {
        [JsonProperty("label")]
        public long Label { get; set; }

        [JsonProperty("start")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Start { get; set; }
    }
}