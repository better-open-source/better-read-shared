namespace BetterRead.Shared.Domain.Book
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