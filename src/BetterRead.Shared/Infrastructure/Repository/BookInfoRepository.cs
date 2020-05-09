using System;
using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Domain.Books;
using BetterRead.Shared.Helpers;
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

        public async Task<BookInfo> GetBookInfoAsync(int bookId) =>
            bookId.PipeForward
                (BuildInfo)
                (await _htmlWeb.LoadFromWebAsync(BookUrl(bookId)));
        
        private static Func<HtmlDocument, BookInfo> BuildInfo(int bookId) =>
            doc =>
                new BookInfo(
                    bookId:   bookId,
                    name:     Extract(doc)($"read_book.php?id={bookId}"),
                    author:   Extract(doc)("author="),
                    url:      BookUrl(bookId),
                    imageUrl: ImageUrl(bookId));
        
        private static Func<string, string> Extract(HtmlDocument document) =>
            predicate => document.DocumentNode
                .QuerySelectorAll("a")
                .Where(n => NodeAttributeValue(n, "href").Contains(predicate))
                .Select(a => NodeAttributeValue(a, "title"))
                .First();

        private static string BookUrl(int bookId) =>
            string.Format(BookUrlPatterns.General, bookId);
        
        private static string ImageUrl(int bookId) =>
            string.Format(BookUrlPatterns.Cover, bookId);
    }
}