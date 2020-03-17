using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BetterRead.Shared.Common.Constants;
using BetterRead.Shared.Domain.Author;
using BetterRead.Shared.Domain.Book;
using BetterRead.Shared.Services.Abstractions;
using BetterRead.Shared.Domain.Search;

namespace BetterRead.Shared.Services
{
    public class SearchService : ISearchService
    {
        private readonly IGetJsonDataService _getJsonDataService;

        public SearchService(IGetJsonDataService getJsonDataService)
        {
            _getJsonDataService = getJsonDataService;
        }

        public async Task<List<Book>> SearchBooksAsync(string bookName)
        {
            var booksData = await Search(bookName, ApiUrls.addressForBooks, SearchPatterns.BookUrl);
            return booksData.Select(book => new Book()
            {
                Info = new BookInfo()
                {
                    Name = book.TitleNoFormatting.Split("- читать")[0],
                    Author = book.TitleNoFormatting.Split("Автор:")[0],
                    Url = book.FormattedUrl
                }
            }).ToList();
        }

        public async Task<List<Author>> SearchAuthorAsync(string authorName)
        {
            var authorsData = await Search(authorName, ApiUrls.addressForAuthors, SearchPatterns.AuthorUrl);
            return authorsData.Select(book => new Author()
            {
                AuthorName = book.TitleNoFormatting.Split("-")[0],
                AuthorId = book.FormattedUrl.Split("=")[1],
            }).ToList();
        }

        public async Task<List<AuthorSeries>> SearchSeriesBookAsync(string seriesName)
        {
            var seriesData = await Search(seriesName, ApiUrls.addressForSeries, SearchPatterns.SeriesUrl);
            return seriesData.Select(series => new AuthorSeries()
            {
                CollectionName = series.TitleNoFormatting,
                CollectionUrl = series.FormattedUrl
            }).ToList();
        }

        private async Task<List<Result>> Search(string name, string address, string urlType)
        {
            var result = await _getJsonDataService.GetDataAsync(name, address);
            return result.Where(rs =>
                rs.FormattedUrl.ToLower().Contains(urlType) &&
                rs.TitleNoFormatting.ToLower().Contains((name).ToLower())).ToList();
        }
    }
}