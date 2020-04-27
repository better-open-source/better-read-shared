namespace BetterRead.Shared.Infrastructure.Domain.Books
{
    public class SheetContent
    {
        public SheetContent(string content, SheetContentType contentType)
        {
            Content = content;
            ContentType = contentType;
        }
        
        public string Content { get; }
        public SheetContentType ContentType { get; }
    }
}