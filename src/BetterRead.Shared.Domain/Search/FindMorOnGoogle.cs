using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class FindMoreOnGoogle
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}