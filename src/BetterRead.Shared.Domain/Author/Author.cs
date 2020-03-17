using BetterRead.Shared.Domain.Book;
namespace BetterRead.Shared.Domain.Author
{
    public class Author
    {
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public Book.Book[] AuthorBooksOutOfSeries { get; set; }
    }
}