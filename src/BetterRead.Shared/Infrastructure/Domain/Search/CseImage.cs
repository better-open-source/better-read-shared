using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Infrastructure.Domain.Search
{
    public class CseImage
    {
        [JsonProperty("src")]
        public Uri Src { get; set; }
    }
}