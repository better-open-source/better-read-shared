using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    internal abstract class BaseRepository
    {
        protected static string NodeAttributeValue(HtmlNode node, string attribute) =>
            node.GetAttributeValue(attribute, string.Empty);
    }
}