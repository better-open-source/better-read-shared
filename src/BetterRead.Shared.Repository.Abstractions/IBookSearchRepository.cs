using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Domain.Book;

namespace BetterRead.Shared.Repository.Abstractions
{
    public interface IBookSearchRepository
    {
        Task<IEnumerable<BookInfo>> SearchBooks(string name);
        Task<IEnumerable<BookInfo>> SearchBooksByName(string name);
        Task<IEnumerable<BookInfo>> SearchAuthors(string author);
        Task<IEnumerable<BookInfo>> SearchBooksBySeries(string series);
    }
}