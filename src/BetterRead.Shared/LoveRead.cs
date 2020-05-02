using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Domain.Books;
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

        /// <summary>
        /// Get generated book docx document
        /// </summary>
        /// <param name="book">Book object</param>
        /// <returns>Document as byte array</returns>
        Task<byte[]> GetBookDocument(Book book);
    }
    
    /// <inheritdoc />
    public class LoveRead : ILoveRead
    {
        private readonly IBookService _bookService;

        /// <summary>
        /// Constructor
        /// There should be registered encoding provider to allow cyrillic encoding  
        /// </summary>
        public LoveRead()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _bookService = ResolveBookService(ResolveHtmlWeb());
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

        /// <inheritdoc />
        public async Task<byte[]> GetBookDocument(Book book) =>
            await BookDocumentBuilder.Build(book);

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