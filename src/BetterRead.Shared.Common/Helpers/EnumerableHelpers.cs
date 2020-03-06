using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterRead.Shared.Common.Helpers
{
    public static class EnumerableHelpers
    {
        public static IEnumerable<IEnumerable<T>> SplitOn<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
            source.Aggregate(new List<List<T>> {new List<T>()},
                (list, value) =>
                {
                    if (predicate(value))
                        list.Add(new List<T>());
                    else
                        list.Last().Add(value);
                    return list;
                });
    }
}