using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Helpers;
using BetterRead.Shared.Infrastructure.Domain.Books;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Infrastructure.Repository
{
    internal interface IBookSheetsRepository
    {
        Task<IEnumerable<Sheet>> GetSheetsAsync(int bookId);
    }
    
    internal class BookSheetsRepository : BaseRepository, IBookSheetsRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookSheetsRepository(HtmlWeb htmlWeb) => 
            _htmlWeb = htmlWeb;

        public async Task<IEnumerable<Sheet>> GetSheetsAsync(int bookId)
        {
            var firstPageNode = await GetHtmlNodeAsync(bookId, 1);
            return Enumerable.Range(1, GetSheetsCount(firstPageNode.Node))
                .Select(i => GetHtmlNodeAsync(bookId, i))
                .WaitAll()
                .Select(t => new Sheet(t.PageNumber, ExtractSheetContent(t.Node)));
        }

        private async Task<(int PageNumber, HtmlNode Node)> GetHtmlNodeAsync(int bookId, int pageNumber)
        {
            var url = string.Format(BookUrlPatterns.Read, bookId, pageNumber);
            var document = await _htmlWeb.LoadFromWebAsync(url);
            return (pageNumber, document.DocumentNode);
        }

        private static IEnumerable<SheetContent> ExtractSheetContent(HtmlNode htmlNode)
        {
            var nodes = htmlNode.QuerySelectorAll("div.MsoNormal").SingleOrDefault()?.ChildNodes;
            if (nodes == null) yield break;

            foreach (var node in nodes)
            {
                if (node.Attributes.Any(attr => attr.Value == "take_h1"))
                    yield return GetHeaderSheetContent(node);

                if (node.Attributes.Any(attr => attr.Value == "MsoNormal"))
                    yield return GetParagraphSheetContent(node);

                if (node.ChildNodes.Any(childNode => childNode.Name == "a"))
                    yield return GetParagraphWithHyperLinkSheetContent(node);

                if (node.Attributes.Any(childNode => childNode.Value.StartsWith("gl")))
                    yield return GetParagraphWithHyperLinkSheetContent(node.Attributes.FirstOrDefault()?.Value);

                if (node.Attributes.Any(attr => attr.Name == "src" && attr.Value.Contains("img/photo_books/")))
                    yield return GetImageSheetContent(node);
            }
        }

        private static SheetContent GetHeaderSheetContent(HtmlNode node) =>
            new SheetContent(node.InnerText, SheetContentType.Header);

        private static SheetContent GetParagraphSheetContent(HtmlNode node) =>
            new SheetContent(node.InnerText, SheetContentType.Paragraph);

        private static SheetContent GetParagraphWithHyperLinkSheetContent(string text) =>
            new SheetContent(text, SheetContentType.HyperLink);

        private static SheetContent GetParagraphWithHyperLinkSheetContent(HtmlNode node) =>
            new SheetContent(node.ChildNodes
                .FirstOrDefault(child =>
                    child.Name == "a" &&
                    child.Attributes.Any(attr => attr.Value.StartsWith("gl")))
                ?.Attributes
                .FirstOrDefault()?.Value ?? "", SheetContentType.HyperLink);

        private static SheetContent GetImageSheetContent(HtmlNode node) => 
            new SheetContent($"{BookUrlPatterns.BaseUrl}/{NodeAttributeValue(node, "src")}", SheetContentType.Image);

        private static int GetSheetsCount(HtmlNode node) =>
            node.QuerySelectorAll("div.navigation > a")
                .Select(n => n.InnerHtml)
                .Where(t => int.TryParse(t, out _))
                .Select(int.Parse)
                .Max();
    }
}