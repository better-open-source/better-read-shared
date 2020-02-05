using System.Collections.Generic;
using System.Threading.Tasks;
using BetterRead.Shared.Domain.Book;

namespace BetterRead.Shared.Services.Abstractions
{
    public interface IBookService
    {
        Task<Book> GetBookByIdAsync(int bookId);
        Task<Book> GetBookByUrlAsync(string url);
        Task<BookInfo> GetBookInfoByIdAsync(int bookId);
        Task<BookInfo> GetBookInfoByUrlAsync(string url);
        Task<IEnumerable<BookInfo>> SearchBooks(string name);
    }
}