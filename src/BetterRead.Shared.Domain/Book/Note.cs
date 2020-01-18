using System.Collections.Generic;

namespace BetterRead.Shared.Domain.Book
{
    public class Note
    {
        public int Id { get; set; }
        public IEnumerable<string> Contents { get; set; }
    }
}