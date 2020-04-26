using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Infrastructure.Domain.Search
{
    public class FindMoreOnGoogle
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}