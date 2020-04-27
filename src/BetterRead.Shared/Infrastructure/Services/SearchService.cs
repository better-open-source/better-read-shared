using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BetterRead.Shared.Constants;
using BetterRead.Shared.Infrastructure.Domain.Api.GoogleSearch;
using BetterRead.Shared.Infrastructure.Domain.Authors;
using BetterRead.Shared.Infrastructure.Domain.Books;

namespace BetterRead.Shared.Infrastructure.Services
{
    internal interface ISearchService
    {
        Task<IEnumerable<BookInfo>> SearchBooksAsync(string bookName);
        Task<IEnumerable<Author>> SearchAuthorAsync(string bookName);
        Task<IEnumerable<AuthorSeries>> SearchSeriesBookAsync(string bookName);
    }
    
    internal class SearchService : ISearchService
    {
        private readonly IGetJsonDataService _getJsonDataService;

        public SearchService(IGetJsonDataService getJsonDataService) => 
            _getJsonDataService = getJsonDataService;

        public async Task<IEnumerable<BookInfo>> SearchBooksAsync(string bookName)
        {
            var results = await Search(bookName, GoogleApiUrls.SearchBooks);
            return results.Select(result => new BookInfo(
                bookId:   GetBookId(result.FormattedUrl),
                name:     result.TitleNoFormatting.Split("- читать")[0],
                author:   result.TitleNoFormatting.Split("Автор:")[0],
                url:      result.FormattedUrl,
                imageUrl: string.Format(BookUrlPatterns.Cover, GetBookId(result.FormattedUrl))));
        }

        public async Task<IEnumerable<Author>> SearchAuthorAsync(string authorName)
        {
            var authorsData = await Search(authorName, GoogleApiUrls.SearchAuthors);
            return authorsData.Select(book => new Author
            {
                AuthorName = book.TitleNoFormatting.Split("-")[0],
                AuthorId = book.FormattedUrl.Split("=")[1],
            });
        }

        public async Task<IEnumerable<AuthorSeries>> SearchSeriesBookAsync(string seriesName)
        {
            var seriesData = await Search(seriesName, GoogleApiUrls.SearchSeries);
            return seriesData.Select(series => new AuthorSeries
            {
                CollectionName = series.TitleNoFormatting,
                CollectionUrl = series.FormattedUrl
            });
        }

        private async Task<IEnumerable<Result>> Search(string name, string address)
        {
            var result = await _getJsonDataService.GetDataAsync(name, address);
            return result.Where(rs =>
                rs.FormattedUrl.ToLower().Contains(BookUrlPatterns.BaseUrl) &&
                rs.TitleNoFormatting.ToLower().Contains(name.ToLower()));
        }
        
        private static int GetBookId(string url)
        {
            var uri = new Uri(url);
            var queryId = HttpUtility.ParseQueryString(uri.Query).Get("id");
            
            return int.Parse(queryId);
        }
    }
}