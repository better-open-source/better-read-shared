using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class CseImage
    {
        [JsonProperty("src")]
        public Uri Src { get; set; }
    }
}