using System.Collections.Generic;

namespace BetterRead.Shared.Infrastructure.Domain.Books
{
    public class Note
    {
        public int Id { get; set; }
        public IEnumerable<string> Contents { get; set; }
    }
}