using System.Collections.Generic;
using System.Threading.Tasks;
using BetterRead.Shared.Domain.Author;
using BetterRead.Shared.Domain.Book;

namespace BetterRead.Shared.Services.Abstractions
{
    public interface ISearchService
    {
        Task<List<Book>> SearchBooksAsync(string bookName);
        Task<List<Author>> SearchAuthorAsync(string bookName);
        Task<List<AuthorSeries>> SearchSeriesBookAsync(string bookName);
    }
}