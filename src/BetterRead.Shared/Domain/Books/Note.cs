using System.Collections.Generic;

namespace BetterRead.Shared.Domain.Books
{
    public class Note
    {
        public int Id { get; set; }
        public IEnumerable<string> Contents { get; set; }
    }
}