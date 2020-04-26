using System.Collections.Generic;

namespace BetterRead.Shared.Infrastructure.Domain.Book
{
    public class Sheet
    {
        public int Id { get; set; }
        public IEnumerable<SheetContent> SheetContents { get; set; }
    }
}