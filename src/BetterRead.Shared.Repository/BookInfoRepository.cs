using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Repository
{
    public class BookInfoRepository : IBookInfoRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookInfoRepository()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _htmlWeb = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("windows-1251")};
        }

        public async Task<BookInfo> GetBookInfoAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.General, bookId);
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var documentNode = htmlDocument.DocumentNode;

            var bookInfo = new BookInfo
            {
                BookId = bookId,
                Url = url,
                Name = GetBookInfoNameFromPage(bookId, documentNode),
                Author = GetBookAuthorFromPage(documentNode),
                ImageUrl = string.Format(BookUrlPatterns.Cover, bookId)
            };
            
            return bookInfo;
        }
        
        private static string GetBookAuthorFromPage(HtmlNode node) =>
            node.QuerySelectorAll("a")
                .First(a => a.GetAttributeValue("href", string.Empty).Contains("author="))
                .GetAttributeValue("title", string.Empty);

        private static string GetBookInfoNameFromPage(int bookId, HtmlNode node) =>
            node.QuerySelectorAll("a")
                .Where(n => n.GetAttributeValue("href", string.Empty).Contains($"read_book.php?id={bookId}"))
                .Select(a => a.GetAttributeValue("title", string.Empty))
                .First();
    }
}