using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class Cursor
    {
        [JsonProperty("currentPageIndex")]
        public long CurrentPageIndex { get; set; }

        [JsonProperty("estimatedResultCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long EstimatedResultCount { get; set; }

        [JsonProperty("moreResultsUrl")]
        public Uri MoreResultsUrl { get; set; }

        [JsonProperty("resultCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ResultCount { get; set; }

        [JsonProperty("searchResultTime")]
        public string SearchResultTime { get; set; }

        [JsonProperty("pages")]
        public Page[] Pages { get; set; }
    }
}