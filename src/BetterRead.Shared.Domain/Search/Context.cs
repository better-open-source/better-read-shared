using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class Context
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("total_results")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalResults { get; set; }
    }
}