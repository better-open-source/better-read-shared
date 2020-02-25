﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Common.Helpers;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BetterRead.Shared.Repository
{
    public class BookSheetsRepository : IBookSheetsRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookSheetsRepository()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _htmlWeb = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("windows-1251")};
        }

        public async Task<IEnumerable<Sheet>> GetSheetsAsync(int bookId)
        {
            var firstPageNode = await GetHtmlNodeAsync(bookId, 1);
            var sheetsCount = GetSheetsCount(firstPageNode.Node);

            var sheets = Enumerable.Range(1, sheetsCount)
                .Select(i => GetHtmlNodeAsync(bookId, i))
                .WaitAll()
                .ToList()
                .Select(t => new Sheet {Id = t.PageNumber, SheetContents = GetNodeContent(t.Node)});

            return sheets;
        }

        private async Task<(int PageNumber, HtmlNode Node)> GetHtmlNodeAsync(int bookId, int pageNumber)
        {
            var url = string.Format(BookUrlPatterns.Read, bookId, pageNumber);
            var document = await _htmlWeb.LoadFromWebAsync(url);
            return (pageNumber, document.DocumentNode);
        }

        private static IEnumerable<SheetContent> GetNodeContent(HtmlNode htmlNode)
        {
            var nodes = htmlNode.QuerySelectorAll("div.MsoNormal").SingleOrDefault()?.ChildNodes;
            if (nodes == null) yield break;

            foreach (var node in nodes)
            {
                if (node.Attributes.Any(attr => attr.Value == "take_h1"))
                    yield return GetHeaderSheetContent(node);

                if (node.Attributes.Any(attr => attr.Value == "MsoNormal"))
                    yield return GetParagraphSheetContent(node);

                if (node.ChildNodes.Any(childNode => childNode.Name == "b"))
                    yield return GetParagraphWithHyperLinkSheetContent(node, "b");

                if (node.ChildNodes.Any(childNode => childNode.Name == "a"))
                    yield return GetParagraphWithHyperLinkSheetContent(node, "a");

                if (node.Attributes.Any(childNode => childNode.Value.StartsWith("gl")))
                    yield return GetParagraphWithHyperLinkSheetContent(node.Attributes.FirstOrDefault()?.Value);

                if (node.Attributes.Any(attr => attr.Name == "src" && attr.Value.Contains("img/photo_books/")))
                    yield return GetImageSheetContent(node, BookUrlPatterns.BaseUrl);
            }
        }

        private static SheetContent GetHeaderSheetContent(HtmlNode node) =>
            new SheetContent(node.InnerText, SheetContentType.Header);

        private static SheetContent GetParagraphSheetContent(HtmlNode node) =>
            new SheetContent(node.InnerText, SheetContentType.Paragraph);

        private static SheetContent GetParagraphWithHyperLinkSheetContent(string text) =>
            new SheetContent(text, SheetContentType.HyperLink);

        private static SheetContent GetParagraphWithHyperLinkSheetContent(HtmlNode node, string type)
        {
            if (type == "a")
                return new SheetContent(node.ChildNodes
                                            .FirstOrDefault(child =>
                                                child.Name == "a" &&
                                                child.Attributes.Any(attr => attr.Value.StartsWith("gl")))?.Attributes
                                            .FirstOrDefault()?.Value ?? "", SheetContentType.HyperLink);
            return new SheetContent(node.ChildNodes
                    .FirstOrDefault(child => child.Name == "b")?.InnerHtml, SheetContentType.HyperLinkNote);
        }

        private static SheetContent GetImageSheetContent(HtmlNode node, string baseUrl)
        {
            var imageQuery = node.Attributes.FirstOrDefault(attr => attr.Name == "src")?.Value;
            return new SheetContent($"{baseUrl}/{imageQuery}", SheetContentType.Image);
        }

        private static int GetSheetsCount(HtmlNode node) =>
            node.QuerySelectorAll("div.navigation > a")
                .Select(n => n.InnerHtml)
                .Where(t => int.TryParse(t, out _))
                .Select(int.Parse)
                .Max();
    }
}