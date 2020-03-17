using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BetterRead.Shared.Services.Abstractions;
using Newtonsoft.Json;
using BetterRead.Shared.Domain.Search;

namespace BetterRead.Shared.Services
{
    public class GetJsonDataService : IGetJsonDataService
    {
        public async Task<List<Result>> GetDataAsync(string searchTerm, string address)
        {
            var url = String.Format(address, 1, searchTerm);
            var pagesCount = await GetPagesCountAsync(url);
            var answer = new Task<CseGoogleResponse>[pagesCount];

            var tasks = answer.Select((value, index) => 
                GetPageAsync(String.Format(address, index, searchTerm))).ToArray();
            
            Task.WaitAll(tasks);
            return tasks.SelectMany(ans => ans.Result.SearchResults).ToList();
        }

        private async Task<CseGoogleResponse> GetPageAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var jsonAnswer = DeleteCallBackName(await client.GetStringAsync(new Uri(url)));
                return JsonConvert.DeserializeObject<CseGoogleResponse>(jsonAnswer);
            }
        }

        private async Task<int> GetPagesCountAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var jsonAnswer = DeleteCallBackName(await client.GetStringAsync(url));
                return JsonConvert.DeserializeObject<CseGoogleResponse>(jsonAnswer).Cursor.Pages.Count();
            }
        }

        private string DeleteCallBackName(string jsonAnswer)
        {
            jsonAnswer = jsonAnswer.Remove(0, jsonAnswer.IndexOf("{", StringComparison.Ordinal));
            return jsonAnswer.Remove(jsonAnswer.LastIndexOf(";", StringComparison.Ordinal) - 1, 2);
        }
    }
}