using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Marvin.Tools
{
    /// <summary>
    /// Static extension class for the <see cref="ObservableCollection{T}"/>
    /// </summary>
    public static class ObservableCollectionExtension
    {
        /// <summary>
        /// Adds a range of items to the observable collection
        /// </summary>
        public static void AddRange<TSource>(this ObservableCollection<TSource> source, IEnumerable<TSource> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        /// <summary>
        /// Removes a range of items from the observable collection
        /// </summary>
        public static void RemoveRange<TSource>(this ObservableCollection<TSource> collection, IEnumerable<TSource> items)
        {
            var itemsArray = new List<TSource>(items).ToArray();
            for (var i = itemsArray.Length - 1; i >= 0; i--)
            {
                collection.Remove(itemsArray[i]);
            }
        }

        /// <summary>
        /// Removes a range of items by the given condition
        /// </summary>
        public static void RemoveBy<TSource>(this ObservableCollection<TSource> collection, Func<TSource, bool> condition)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Replaces the first item which mets the given condition
        /// </summary>
        public static void ReplaceItem<TSource>(this Collection<TSource> col, Func<TSource, bool> match, TSource newItem)
        {
            var oldItem = col.FirstOrDefault(match);
            if (oldItem == null) 
                return;

            var oldIndex = col.IndexOf(oldItem);
            if (oldIndex != -1)
            {
                col[oldIndex] = newItem;
            }
        }
    }
}
