using System.Collections.Generic;
using System.Threading.Tasks;
using BetterRead.Shared.Domain.Search;
using BetterRead.Shared.Domain.Search;

namespace BetterRead.Shared.Services.Abstractions
{
    public interface IGetJsonDataService
    {
        Task<List<Result>> GetDataAsync(string searchTerm, string address);
    }
}