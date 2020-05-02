using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Api.GoogleSearch
{
    public class CseImage
    {
        [JsonProperty("src")]
        public Uri Src { get; set; }
    }
}