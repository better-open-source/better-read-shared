using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Infrastructure.Domain.Books;
using BetterRead.Shared.Infrastructure.Repository;
using BetterRead.Shared.Infrastructure.Services;
using HtmlAgilityPack;

namespace BetterRead.Shared
{
    /// <summary>
    /// LoveRead Abstraction
    /// </summary>
    public interface ILoveRead
    {
        /// <summary>
        /// Get book by book id
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <returns>Book</returns>
        Task<Book> GetBookAsync(int bookId);
        
        /// <summary>
        /// Get book by book url 
        /// </summary>
        /// <param name="url">Book url</param>
        /// <returns>Book</returns>
        Task<Book> GetBookAsync(string url);
        
        /// <summary>
        /// Get book info by book id
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <returns>BookInfo</returns>
        Task<BookInfo> GetBookInfoAsync(int bookId);
        
        /// <summary>
        /// Get book info by book url
        /// </summary>
        /// <param name="url">Book url</param>
        /// <returns>BookInfo</returns>
        Task<BookInfo> GetBookInfoAsync(string url);
    }
    
    /// <inheritdoc />
    public class LoveRead : ILoveRead
    {
        private readonly IBookService _bookService;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoveRead()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var web = ResolveHtmlWeb();
            _bookService = ResolveBookService(web);
        }

        /// <inheritdoc />
        public async Task<Book> GetBookAsync(int bookId) => 
            await _bookService.GetBookByIdAsync(bookId);

        /// <inheritdoc />
        public async Task<Book> GetBookAsync(string url) => 
            await _bookService.GetBookByUrlAsync(url);

        /// <inheritdoc />
        public async Task<BookInfo> GetBookInfoAsync(int bookId) => 
            await _bookService.GetBookInfoByIdAsync(bookId);

        /// <inheritdoc />
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