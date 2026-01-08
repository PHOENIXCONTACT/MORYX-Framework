// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools;

/// <summary>
/// ICollection Extensions
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds a range of items to the collection
    /// </summary>
    public static void AddRange<TSource>(this ICollection<TSource> source, IEnumerable<TSource> items) where TSource : class
    {
        foreach (var item in items)
            source.Add(item);
    }

    /// <summary>
    /// Replaces the first item which mets the given condition
    /// </summary>
    public static void ReplaceItem<TSource>(this IList<TSource> list, Func<TSource, bool> match, TSource newItem)
    {
        var oldItem = list.FirstOrDefault(match);
        if (oldItem == null)
            return;

        var oldIndex = list.IndexOf(oldItem);
        if (oldIndex != -1)
            list[oldIndex] = newItem;
    }

    extension<TSource>(ICollection<TSource> collection) where TSource : class
    {
        /// <summary>
        /// Removes a range of items from the collection
        /// </summary>
        public void RemoveRange(IEnumerable<TSource> items)
        {
            var itemsArray = new List<TSource>(items).ToArray();
            for (var i = itemsArray.Length - 1; i >= 0; i--)
                collection.Remove(itemsArray[i]);
        }

        /// <summary>
        /// Add an entry only if it does not exist already
        /// </summary>
        public void AddIfNotExists(TSource item)
        {
            if (!collection.Contains(item))
                collection.Add(item);
        }
    }

    extension<TSource>(IList<TSource> collection)
    {
        /// <summary>
        /// Removes a range of items by the given condition
        /// </summary>
        public void RemoveBy(Func<TSource, bool> condition)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                    collection.RemoveAt(i);
            }
        }

        /// <summary>
        /// Shuffles the list
        /// </summary>
        public void Shuffle()
        {
            var rng = new Random();

            var n = collection.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = collection[k];
                collection[k] = collection[n];
                collection[n] = value;
            }
        }
    }
}