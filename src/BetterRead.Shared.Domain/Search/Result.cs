using System;
using Newtonsoft.Json;

namespace BetterRead.Shared.Domain.Search
{
    public class Result
    {
        [JsonProperty("cacheUrl")]
        public Uri CacheUrl { get; set; }

        [JsonProperty("clicktrackUrl")]
        public Uri ClicktrackUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contentNoFormatting")]
        public string ContentNoFormatting { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("titleNoFormatting")]
        public string TitleNoFormatting { get; set; }

        [JsonProperty("formattedUrl")]
        public string FormattedUrl { get; set; }

        [JsonProperty("unescapedUrl")]
        public Uri UnescapedUrl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("visibleUrl")]
        public string VisibleUrl { get; set; }

        [JsonProperty("richSnippet")]
        public RichSnippet RichSnippet { get; set; }
    }

}