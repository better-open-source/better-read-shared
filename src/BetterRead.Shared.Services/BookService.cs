using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository;
using BetterRead.Shared.Repository.Abstractions;
using BetterRead.Shared.Services.Abstractions;

namespace BetterRead.Shared.Services
{
    public class BookService : IBookService
    {
        private readonly IBookInfoRepository _infoRepository;
        private readonly IBookSheetsRepository _sheetsRepository;
        private readonly IBookContentsRepository _contentsRepository;
        private readonly IBookNotesRepository _notesRepository;
        private readonly IBookSearchRepository _bookSearchRepository;

        public BookService()
            : this(new BookInfoRepository(), new BookSheetsRepository(), new BookContentsRepository(),
                new BookNotesRepository(), new BookSearchRepository())
        {
        }

        public BookService(
            IBookInfoRepository infoRepository,
            IBookSheetsRepository sheetsRepository,
            IBookContentsRepository contentsRepository,
            IBookNotesRepository notesRepository,
            IBookSearchRepository bookSearchRepository)
        {
            _sheetsRepository = sheetsRepository ?? throw new ArgumentNullException(nameof(sheetsRepository));
            _infoRepository = infoRepository ?? throw new ArgumentNullException(nameof(infoRepository));
            _contentsRepository = contentsRepository ?? throw new ArgumentNullException(nameof(contentsRepository));
            _notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
            _bookSearchRepository = bookSearchRepository ?? throw new ArgumentNullException(nameof(bookSearchRepository));
        }

        public async Task<Book> GetBookByIdAsync(int bookId) => 
            await GetBookAsync(bookId);

        public async Task<Book> GetBookByUrlAsync(string url) => 
            await GetBookAsync(GetBookId(url));

        public async Task<BookInfo> GetBookInfoByIdAsync(int bookId) => 
            await _infoRepository.GetBookInfoAsync(bookId);

        public async Task<BookInfo> GetBookInfoByUrlAsync(string url) => 
            await _infoRepository.GetBookInfoAsync(GetBookId(url));

        public async Task<IEnumerable<BookInfo>> SearchBooks(string name)=>
            await _bookSearchRepository.SearchBooks(name);

        public async Task<IEnumerable<BookInfo>> SearchBooksByName(string name) =>
            await _bookSearchRepository.SearchBooksByName(name);

        public async Task<IEnumerable<BookInfo>> SearchAuthors(string author) =>
            await _bookSearchRepository.SearchAuthors(author);

        public async Task<IEnumerable<BookInfo>> SearchBooksBySeries(string series) =>
            await _bookSearchRepository.SearchBooksBySeries(series);

        private static int GetBookId(string url)
        {
            var uri = new Uri(url);
            var queryId = HttpUtility.ParseQueryString(uri.Query).Get("id");
            
            return int.Parse(queryId);
        }
        
        private async Task<Book> GetBookAsync(int bookId) =>
            new Book
            {
                Info = await _infoRepository.GetBookInfoAsync(bookId),
                Sheets = await _sheetsRepository.GetSheetsAsync(bookId),
                Contents = await _contentsRepository.GetContentsAsync(bookId),
                //Notes = await _notesRepository.GetNotesAsync(bookId)
            };
    }
}