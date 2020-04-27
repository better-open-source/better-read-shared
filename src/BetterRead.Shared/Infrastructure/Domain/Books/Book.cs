using System.Collections.Generic;

namespace BetterRead.Shared.Infrastructure.Domain.Books
{
    public class Book
    {
        public Book(BookInfo info, IEnumerable<Sheet> sheets, IEnumerable<Content> contents, IEnumerable<Note> notes)
        {
            Info = info;
            Sheets = sheets;
            Contents = contents;
            Notes = notes;
        }
        
        public int Id => Info.BookId;
        public BookInfo Info { get; }
        public IEnumerable<Sheet> Sheets { get; }
        public IEnumerable<Content> Contents { get; }
        public IEnumerable<Note> Notes { get; }
    }
}