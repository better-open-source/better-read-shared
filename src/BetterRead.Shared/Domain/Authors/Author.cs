using System.Collections.Generic;
using BetterRead.Shared.Domain.Books;

namespace BetterRead.Shared.Domain.Authors
{
    public class Author
    {
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public IEnumerable<Book> AuthorBooksOutOfSeries { get; set; }
    }
}