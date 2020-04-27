using Newtonsoft.Json;

namespace BetterRead.Shared.Infrastructure.Domain.Api.GoogleSearch
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