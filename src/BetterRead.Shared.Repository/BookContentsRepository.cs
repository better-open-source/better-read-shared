using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Repository
{
    public class BookContentsRepository : IBookContentsRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookContentsRepository()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _htmlWeb = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("windows-1251")};
        }

        public async Task<IEnumerable<Content>> GetContentsAsync(int bookId)
        {
            var url = string.Format(BookUrlPatterns.Contents, bookId);
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);

            return GetContentsFromNode(htmlDocument.DocumentNode);
        }

        private static IEnumerable<Content> GetContentsFromNode(HtmlNode node) =>
            node.QuerySelectorAll("#oglav_link > li > a")
                .Select(GetContentFromNode);

        private static Content GetContentFromNode(HtmlNode node) =>
            new Content
            {
                Link = node.GetAttributeValue("href", string.Empty),
                Text = node.InnerText
            };
    }
}