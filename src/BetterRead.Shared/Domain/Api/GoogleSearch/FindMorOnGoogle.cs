using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Api.GoogleSearch
{
    public class FindMoreOnGoogle
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}