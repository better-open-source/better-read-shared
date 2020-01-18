using System.Threading.Tasks;
using BetterRead.Shared.Domain.Book;

namespace BetterRead.Shared.Abstractions
{
    public interface ILoveRead
    {
        Task<Book> GetBookAsync(int bookId);
        Task<Book> GetBookAsync(string url);
        Task<BookInfo> GetBookInfoAsync(int bookId);
        Task<BookInfo> GetBookInfoAsync(string url);
    }
}