using System;
using System.Collections.Generic;

namespace BetterRead.Shared.Helpers
{
    // Extensions is misspelled. Try install spell-checker
    internal static class FunctionalExtenstions
    {
        public static TRes PipeForward<TArg, TRes>(
            this TArg arg,
            Func<TArg, TRes> func)
        {
            return func(arg);
        }
        
        public static void PipeForward<TArg>(this TArg arg, Action<TArg> action)
        {
            action(arg);
        }
        
        public static Func<TArg2, TRes> Curry<TArg1, TArg2, TRes>(
            this Func<TArg1, TArg2, TRes> func, 
            TArg1 arg1)
        {
            return arg2 => func(arg1, arg2);
        }

        public static Action<TArg2> Curry<TArg1, TArg2>(
            this Action<TArg1, TArg2> action, 
            TArg1 arg1)
        {
            return arg2 => action(arg1, arg2);
        }

        public static IEnumerable<T> With<T>(
            this IEnumerable<T> collection,
            IEnumerable<T> withCollection)
        {
            var enumerable = new List<T>(collection);
            enumerable.AddRange(withCollection);
            return enumerable;
        }
        
        public static TRes Apply<TArg, TRes>(
            this Func<TArg, TRes> func,
            TArg arg)
        {
            return func(arg);
        }

        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) 
                action(item);
        }
    }
}