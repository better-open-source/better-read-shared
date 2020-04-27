using System;
using System.Threading.Tasks;
using System.Web;
using BetterRead.Shared.Infrastructure.Domain.Books;
using BetterRead.Shared.Infrastructure.Repository;

namespace BetterRead.Shared.Infrastructure.Services
{
    internal interface IBookService
    {
        Task<Book> GetBookByIdAsync(int bookId);
        Task<Book> GetBookByUrlAsync(string url);
        Task<BookInfo> GetBookInfoByIdAsync(int bookId);
        Task<BookInfo> GetBookInfoByUrlAsync(string url);
    }
    
    internal class BookService : IBookService
    {
        private readonly IBookInfoRepository _infoRepository;
        private readonly IBookSheetsRepository _sheetsRepository;
        private readonly IBookContentsRepository _contentsRepository;
        private readonly IBookNotesRepository _notesRepository;

        public BookService(
            IBookInfoRepository infoRepository,
            IBookSheetsRepository sheetsRepository,
            IBookContentsRepository contentsRepository,
            IBookNotesRepository notesRepository)
        {
            _sheetsRepository = sheetsRepository;
            _infoRepository = infoRepository;
            _contentsRepository = contentsRepository;
            _notesRepository = notesRepository;
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

        private async Task<Book> GetBookAsync(int bookId) =>
            new Book(
                await _infoRepository.GetBookInfoAsync(bookId),
                await _sheetsRepository.GetSheetsAsync(bookId),
                await _contentsRepository.GetContentsAsync(bookId),
                await _notesRepository.GetNotesAsync(bookId));
    }
}