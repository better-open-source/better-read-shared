using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class RichSnippet
    {
        [JsonProperty("cseImage")]
        public CseImage CseImage { get; set; }

        [JsonProperty("cseThumbnail")]
        public CseThumbnail CseThumbnail { get; set; }
    }
}