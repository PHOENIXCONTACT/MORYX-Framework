using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Tools
{
    /// <summary>
    /// ICollection Extensions
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static void AddRange<TSource>(this ICollection<TSource> source, IEnumerable<TSource> items) where TSource : class
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RemoveRange<TSource>(this ICollection<TSource> collection, IEnumerable<TSource> items) where TSource : class
        {
            var itemsArray = new List<TSource>(items).ToArray();
            for (int i = itemsArray.Length - 1; i >= 0; i--)
            {
                collection.Remove(itemsArray[i]);
            }
        }

        /// <summary>
        /// Add an entry only if it does not exist allready
        /// </summary>
        public static void AddIfNotExists<TSource>(this ICollection<TSource> coll, TSource item) where TSource : class
        {
            if (!coll.Contains(item))
                coll.Add(item);
        }

        /// <summary>
        /// Return all entries from the source collection that do not match the filter for any entry in the second collection
        /// </summary>
        public static IEnumerable<TSource> Except<TSource, TCompare>(this IEnumerable<TSource> source, ICollection<TCompare> compare, Func<TSource, TCompare, bool> filter)
        {
            return source.Where(entry => compare.All(item => !filter(entry, item)));
        }

        /// <summary>
        /// Return all entries from the source collection that do match the filter for any entry in the second collection
        /// </summary>
        public static IEnumerable<TSource> Intersect<TSource, TCompare>(this IEnumerable<TSource> source, ICollection<TCompare> compare, Func<TSource, TCompare, bool> filter)
        {
            return source.Where(entry => compare.Any(item => filter(entry, item)));
        }
    }
}
