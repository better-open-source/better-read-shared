using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Infrastructure.Domain.Book;
using BetterRead.Shared.Infrastructure.Repository;
using BetterRead.Shared.Infrastructure.Services;
using HtmlAgilityPack;

namespace BetterRead.Shared
{
    public interface ILoveRead
    {
        Task<Book> GetBookAsync(int bookId);
        Task<Book> GetBookAsync(string url);
        Task<BookInfo> GetBookInfoAsync(int bookId);
        Task<BookInfo> GetBookInfoAsync(string url);
    }
    
    public class LoveRead : ILoveRead
    {
        private readonly IBookService _bookService;

        public LoveRead()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var web = ResolveHtmlWeb();
            _bookService = ResolveBookService(web);
        }

        public async Task<Book> GetBookAsync(int bookId) => 
            await _bookService.GetBookByIdAsync(bookId);

        public async Task<Book> GetBookAsync(string url) => 
            await _bookService.GetBookByUrlAsync(url);

        public async Task<BookInfo> GetBookInfoAsync(int bookId) => 
            await _bookService.GetBookInfoByIdAsync(bookId);

        public async Task<BookInfo> GetBookInfoAsync(string url) => 
            await _bookService.GetBookInfoByUrlAsync(url);
        
        private static HtmlWeb ResolveHtmlWeb() => 
            new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("windows-1251")};
        
        private static BookService ResolveBookService(HtmlWeb web) =>
            new BookService(
                new BookInfoRepository(web),
                new BookSheetsRepository(web),
                new BookContentsRepository(web),
                new BookNotesRepository(web));
    }
}