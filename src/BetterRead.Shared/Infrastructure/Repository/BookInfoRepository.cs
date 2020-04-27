using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Infrastructure.Domain.Books;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    internal interface IBookInfoRepository
    {
        Task<BookInfo> GetBookInfoAsync(int bookId);
    }
    
    internal class BookInfoRepository : BaseRepository, IBookInfoRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookInfoRepository(HtmlWeb htmlWeb) => 
            _htmlWeb = htmlWeb;

        public async Task<BookInfo> GetBookInfoAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.General, bookId);
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var documentNode = htmlDocument.DocumentNode;

            return new BookInfo(
                bookId:   bookId, 
                name:     ExtractName(bookId, documentNode), 
                author:   ExtractAuthor(documentNode),
                url:      url, 
                imageUrl: string.Format(BookUrlPatterns.Cover, bookId));
        }

        private static string ExtractName(int bookId, HtmlNode node) =>
            node.QuerySelectorAll("a")
                .Where(n => NodeAttributeValue(n, "href").Contains($"read_book.php?id={bookId}"))
                .Select(a => NodeAttributeValue(a, "title"))
                .First();        

        private static string ExtractAuthor(HtmlNode node) =>
            node.QuerySelectorAll("a")
                .First(a => NodeAttributeValue(a, "href").Contains("author="))
                .GetAttributeValue("title", string.Empty);
    }
}