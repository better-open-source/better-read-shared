using System;
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

        public BookService()
            : this(new BookInfoRepository(), new BookSheetsRepository(), new BookContentsRepository(),
                new BookNotesRepository())
        {
        }

        public BookService(
            IBookInfoRepository infoRepository,
            IBookSheetsRepository sheetsRepository,
            IBookContentsRepository contentsRepository,
            IBookNotesRepository notesRepository)
        {
            _sheetsRepository = sheetsRepository ?? throw new ArgumentNullException(nameof(sheetsRepository));
            _infoRepository = infoRepository ?? throw new ArgumentNullException(nameof(infoRepository));
            _contentsRepository = contentsRepository ?? throw new ArgumentNullException(nameof(contentsRepository));
            _notesRepository = notesRepository ?? throw new ArgumentNullException(nameof(notesRepository));
        }

        public async Task<Book> GetBookByIdAsync(int bookId) =>
            await GetBookAsync(bookId);

        public async Task<Book> GetBookByUrlAsync(string url) =>
            await GetBookAsync(GetBookId(url));

        public async Task<BookInfo> GetBookInfoByIdAsync(int bookId) =>
            await _infoRepository.GetBookInfoAsync(bookId);

        public async Task<BookInfo> GetBookInfoByUrlAsync(string url) =>
            await _infoRepository.GetBookInfoAsync(GetBookId(url));

        private static int GetBookId(string url)
        {
            var uri = new Uri(url);
            var queryId = HttpUtility.ParseQueryString(uri.Query).Get("id");

            return int.Parse(queryId);
        }

        private async Task<Book> GetBookAsync(int bookId)
        {
            var info = _infoRepository.GetBookInfoAsync(bookId);
            var sheets = _sheetsRepository.GetSheetsAsync(bookId);
            var contents = _contentsRepository.GetContentsAsync(bookId);
            var notes = _notesRepository.GetNotesAsync(bookId);

            await Task.WhenAll(info, sheets, contents);

            return new Book
            {
                Info = await info,
                Sheets = await sheets,
                Contents = await contents,
                Notes = await notes

            };
        }
    }
}