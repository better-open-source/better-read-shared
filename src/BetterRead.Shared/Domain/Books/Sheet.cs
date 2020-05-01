using System.Collections.Generic;

namespace BetterRead.Shared.Domain.Books
{
    public class Sheet
    {
        public Sheet(int id, IEnumerable<SheetContent> sheetContents)
        {
            Id = id;
            SheetContents = sheetContents;
        }
        
        public int Id { get; }
        public IEnumerable<SheetContent> SheetContents { get; }
    }
}