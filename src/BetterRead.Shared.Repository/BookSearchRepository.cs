using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Common.Helpers;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Repository.Abstractions;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace BetterRead.Shared.Repository
{
    public class BookSearchRepository : IBookSearchRepository
    {
        private readonly HtmlWeb _htmlWeb;

        public BookSearchRepository()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _htmlWeb = new HtmlWeb { OverrideEncoding = Encoding.GetEncoding("windows-1251") };
        }

        #region Method loveread.me
      

        public async Task<IEnumerable<BookInfo>> SearchBooksByName(string name)
        {
            var url = string.Format(BookUrlPatterns.SearchByName, name);

            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var books = GetBooksByName(htmlDocument.DocumentNode);

            return books;
        }

        public async Task<IEnumerable<BookInfo>> SearchAuthors(string author)
        {
            var url = string.Format(BookUrlPatterns.SearchAuthor, author);

            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var authors = GetAuthors(htmlDocument.DocumentNode);

            return authors;
        }

        public async Task<IEnumerable<BookInfo>> SearchBooksBySeries(string series)
        {
            var url = string.Format(BookUrlPatterns.SearchBySeries, series);

            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var serieses = GetSerieses(htmlDocument.DocumentNode);

            return serieses;
        }
        #endregion

        #region Generate List loveread.me
        private static IEnumerable<BookInfo> GetBooksByName(HtmlNode node) =>
            node.QuerySelectorAll("div.bodyUnit")
                .Select(GetBookByName);

        private static IEnumerable<BookInfo> GetAuthors(HtmlNode node) =>
            node.QuerySelectorAll("div.bodyUnit")
                .Select(GetAuthor);

        private static IEnumerable<BookInfo> GetSerieses(HtmlNode node) =>
            node.QuerySelectorAll("div.seriesBooks")
                .Select(GetSeries);

        #endregion

        #region Generate Object loveread.me
        private static BookInfo GetBookByName(HtmlNode node) =>
            new BookInfo
            {
                ImageUrl = GetImage(node),
                Name = GetName(node),
                Url = GetUrl(node),
                BookId = GetBookId(GetUrl(node))
            };

        private static BookInfo GetAuthor(HtmlNode node) =>
            new BookInfo
            {
                ImageUrl = GetImage(node),
                Url = GetUrl(node),
            };

        private static BookInfo GetSeries(HtmlNode node) =>
            new BookInfo
            {
                //  ImageUrl = GetImage(node),
                Name = GetSeriesName(node),
                Url = GetSeriesUrl(node),
                BookId = GetBookId(GetSeriesUrl(node))
            };

        #endregion

        #region Scraping Method loveread.me
        private static string GetImage(HtmlNode node) =>
            node.QuerySelector("img").GetAttributeValue("src", string.Empty);

        private static string GetName(HtmlNode node) =>
            node.QuerySelector("div.openBook > a").InnerText.Trim();

        private static string GetUrl(HtmlNode node) =>
            $"{BookUrlPatterns.SecondBaseUrl}/" +
            $"{node.QuerySelector("div.shortDescription > div > a").GetAttributeValue("href", string.Empty)}";

        private static string GetSeriesName(HtmlNode node) =>
            node.QuerySelector("a").InnerText.Trim();

        private static string GetSeriesUrl(HtmlNode node) =>
            $"{BookUrlPatterns.SecondBaseUrl}/" +
            $"{node.QuerySelector("a").GetAttributeValue("href", string.Empty)}";

        #endregion

        #region Method loveread.ec
        public async Task<IEnumerable<BookInfo>> SearchBooks(string name)
        {
            var url = string.Format(BookUrlPatterns.Search, EncodingHelpers.GetCyrillicEncoding(name));

            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            var books = GetBooks(htmlDocument.DocumentNode);

            return books;
        }
        private static IEnumerable<BookInfo> GetBooks(HtmlNode node) =>
            node
                .QuerySelector("ul.let_ul")
                .QuerySelectorAll("li[style]")
                .Select(GetContentFromNode);


        private static BookInfo GetContentFromNode(HtmlNode node) =>
            new BookInfo
            {
                Name = GetBookName(node),
                Url = GetBookUrl(node),
                Author = GetBookAuthor(node),
                BookId = GetBookId(GetBookUrl(node))
            };

        private static string GetBookName(HtmlNode node) =>
            node.QuerySelectorAll("a").FirstOrDefault()?.InnerText.Trim();

        private static string GetBookUrl(HtmlNode node) =>
            $"{BookUrlPatterns.BaseUrl}/" +
            $"{node.QuerySelectorAll("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty)}";

        private static string GetBookAuthor(HtmlNode node) =>
            node.QuerySelectorAll("a").LastOrDefault()?.InnerText.Trim();

        private static int GetBookId(string url)
        {
            var uri = new Uri(url);
            var queryId = HttpUtility.ParseQueryString(uri.Query).Get("id");

            return int.Parse(queryId);
        }
        #endregion
    }
}
