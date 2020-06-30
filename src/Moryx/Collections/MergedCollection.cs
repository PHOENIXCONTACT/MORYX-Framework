// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Moryx.Collections
{
    /// <summary>
    /// This collection will merge two different <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of first collection.</typeparam>
    /// <typeparam name="TK">The type of the second collection.</typeparam>
    public class MergedCollection<T, TK> : IEnumerable, INotifyCollectionChanged
    {
        readonly ObservableCollection<T> _first;
        readonly ObservableCollection<TK> _second;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedCollection{T, TK}"/> class.
        /// </summary>
        public MergedCollection(ObservableCollection<T> first, ObservableCollection<TK> second)
        {
            _first = first;
            _second = second;

            _first.CollectionChanged += FirstCollectionChanged;
            _second.CollectionChanged += SecondCollectionChanged;
        }

        private void FirstCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs correctedNotifyEventArgs;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    correctedNotifyEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems[0], e.NewStartingIndex + _second.Count);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    correctedNotifyEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.OldItems[0], e.OldStartingIndex + _second.Count);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    correctedNotifyEventArgs = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems[0], e.OldItems[0], e.NewStartingIndex + _second.Count);
                    break;
                default:
                    correctedNotifyEventArgs = e;
                    break;
            }

            CollectionChanged(this, correctedNotifyEventArgs);
        }

        private void SecondCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged(this, e);
        }

        /// <see cref="IEnumerable"/>
        public IEnumerator GetEnumerator()
        {
            foreach (var entry in _second)
            {
                yield return entry;
            }

            foreach (var entry in _first)
            {
                yield return entry;
            }
        }

        /// <see cref="INotifyCollectionChanged"/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
