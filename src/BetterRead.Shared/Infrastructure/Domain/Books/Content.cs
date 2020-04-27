namespace BetterRead.Shared.Infrastructure.Domain.Books
{
    public class Content
    {
        public Content(string link, string text)
        {
            Link = link;
            Text = text;
        }
        
        public string Link { get; }
        public string Text { get; }
    }
}