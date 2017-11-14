using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Marvin.Tools
{
    /// <summary>
    /// Extensions for th <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts an IEnumerable to an observable collection
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> en)
        {
            return en == null ? new ObservableCollection<T>() : new ObservableCollection<T>(en);
        }

        /// <summary>
        /// Flatts a tree of objects. Sample: root.Flatten(c => c.Children)
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
    }
}