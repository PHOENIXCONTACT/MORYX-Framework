﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Moryx.Tools
{
    /// <summary>
    /// Extensions for th <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts an IEnumerable to an observable collection
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? new ObservableCollection<T>() : new ObservableCollection<T>(enumerable);
        }

        /// <summary>
        /// Flats a tree of objects. Sample: root.Flatten(c => c.Children)
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }

        /// <summary>
        /// Executes an action for each element of an <see cref="IEnumerable{T}"/>
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }
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
