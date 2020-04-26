using System.Collections.Generic;

namespace BetterRead.Shared.Infrastructure.Domain.Book
{
    public class Note
    {
        public int Id { get; set; }
        public IEnumerable<string> Contents { get; set; }
    }
}