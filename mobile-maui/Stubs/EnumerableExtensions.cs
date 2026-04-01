using System;
using System.Collections.Generic;
using System.Linq;

namespace BioBrain.Extensions
{
    /// <summary>
    /// Extension methods that were available in Xamarin.Forms.Internals but not in MAUI.
    /// </summary>
    public static class EnumerableCompatExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Returns the index of the first element matching the predicate, or -1 if none found.
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item)) return index;
                index++;
            }
            return -1;
        }
    }
}
