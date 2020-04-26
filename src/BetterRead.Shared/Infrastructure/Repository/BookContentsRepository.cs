using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Infrastructure.Domain.Book;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    public interface IBookContentsRepository
    {
        Task<IEnumerable<Content>> GetContentsAsync(int bookId);
    }
    
    public class BookContentsRepository : IBookContentsRepository
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
                .Select(GetContentFromNode);

        private static Content GetContentFromNode(HtmlNode node) =>
            new Content
            {
                Link = node.GetAttributeValue("href", string.Empty),
                Text = node.InnerText
            };
    }
}