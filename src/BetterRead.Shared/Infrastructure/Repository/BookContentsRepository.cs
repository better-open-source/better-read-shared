using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Infrastructure.Domain.Books;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    internal interface IBookContentsRepository
    {
        Task<IEnumerable<Content>> GetContentsAsync(int bookId);
    }
    
    internal class BookContentsRepository : BaseRepository, IBookContentsRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookContentsRepository(HtmlWeb htmlWeb) => 
            _htmlWeb = htmlWeb;

        public async Task<IEnumerable<Content>> GetContentsAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.Contents, bookId);
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);

            return GetContentsFromNode(htmlDocument.DocumentNode);
        }

        private static IEnumerable<Content> GetContentsFromNode(HtmlNode node) =>
            node.QuerySelectorAll("#oglav_link > li > a")
                .Select(MapNodeToContent);

        private static Content MapNodeToContent(HtmlNode node) =>
            new Content(NodeAttributeValue(node, "href"), node.InnerText);
    }
}