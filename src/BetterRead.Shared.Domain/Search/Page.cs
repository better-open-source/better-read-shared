using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
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