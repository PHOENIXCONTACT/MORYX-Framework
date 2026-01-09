// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.ObjectModel;
using Moryx.Serialization;

namespace Moryx.Tools;

/// <summary>
/// Extensions for th <see cref="IEnumerable{T}"/>
/// </summary>
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Converts an IEnumerable to an observable collection
        /// </summary>
        public ObservableCollection<T> ToObservableCollection()
        {
            return enumerable == null ? [] : new ObservableCollection<T>(enumerable);
        }

        /// <summary>
        /// Flats a tree of objects. Sample: root.Flatten(c => c.Children)
        /// </summary>
        public IEnumerable<T> Flatten(Func<T, IEnumerable<T>> f)
        {
            return enumerable.SelectMany(c => f(c).Flatten(f)).Concat(enumerable);
        }

        /// <summary>
        /// Executes an action for each element of an <see cref="IEnumerable{T}"/>
        /// </summary>
        public void ForEach(Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        /// <summary>
        /// Return all entries from the source collection that do not match the filter for any entry in the second collection
        /// </summary>
        public IEnumerable<T> Except<TCompare>(ICollection<TCompare> compare, Func<T, TCompare, bool> filter)
        {
            return enumerable.Where(entry => compare.All(item => !filter(entry, item)));
        }

        /// <summary>
        /// Return all entries from the source collection that do match the filter for any entry in the second collection
        /// </summary>
        public IEnumerable<T> Intersect<TCompare>(ICollection<TCompare> compare, Func<T, TCompare, bool> filter)
        {
            return enumerable.Where(entry => compare.Any(item => filter(entry, item)));
        }
    }
}