using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BetterRead.Shared.Helpers;
using BetterRead.Shared.Infrastructure.Domain.Api.GoogleSearch;
using Newtonsoft.Json;

namespace BetterRead.Shared.Infrastructure.Services
{
    internal interface IGetJsonDataService
    {
        Task<IEnumerable<Result>> GetDataAsync(string searchTerm, string address);
    }
    
    internal class GetJsonDataService : IGetJsonDataService
    {
        public async Task<IEnumerable<Result>> GetDataAsync(string searchTerm, string address)
        {
            var url = string.Format(address, 1, searchTerm);
            var pagesCount = await GetPagesCountAsync(url);
            var answer = new Task<CseGoogleResponse>[pagesCount];

            return answer
                .Select((value, index) => GetPageAsync(string.Format(address, index, searchTerm)))
                .WaitAll()
                .SelectMany(ans => ans.SearchResults);
        }

        private static async Task<CseGoogleResponse> GetPageAsync(string url)
        {
            using var client = new HttpClient();
            var jsonAnswer = DeleteCallBackName(await client.GetStringAsync(new Uri(url)));
            return JsonConvert.DeserializeObject<CseGoogleResponse>(jsonAnswer);
        }

        private static async Task<int> GetPagesCountAsync(string url)
        {
            using var client = new HttpClient();
            var jsonAnswer = DeleteCallBackName(await client.GetStringAsync(url));
            return JsonConvert.DeserializeObject<CseGoogleResponse>(jsonAnswer).Cursor.Pages.Count();
        }

        private static string DeleteCallBackName(string jsonAnswer)
        {
            jsonAnswer = jsonAnswer.Remove(0, jsonAnswer.IndexOf("{", StringComparison.Ordinal));
            return jsonAnswer.Remove(jsonAnswer.LastIndexOf(";", StringComparison.Ordinal) - 1, 2);
        }
    }
}