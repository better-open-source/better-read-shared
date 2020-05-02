using System.Collections.Generic;
using System.Threading.Tasks;

namespace BetterRead.Shared.Helpers
{
    public static class TaskHelpers
    {
        public static IEnumerable<T> WaitAll<T>(this IEnumerable<Task<T>> source, bool continueOnCapturedContext = false)
        {
            return Task.WhenAll(source).ConfigureAwait(continueOnCapturedContext).GetAwaiter().GetResult();
        }
    }
}