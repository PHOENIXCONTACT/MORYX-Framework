// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Moryx.Serialization
{
    /// <summary>
    /// Collection type for typed representations of config collections
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection</typeparam>
    public class EntryCollection<T> : EntryCollectionBase<T>
        where T : class, new()
    {
        /// <summary>
        /// Internally we wrap an observable collection
        /// </summary>
        private readonly IList<ModelAndEntry> _internalCollection = new List<ModelAndEntry>();

        /// <summary>
        /// Create a new instance of the collection
        /// </summary>
        /// <param name="collectionRoot"></param>
        public EntryCollection(Entry collectionRoot) : base(collectionRoot)
        {
            foreach (var subEntry in collectionRoot.SubEntries)
            {
                Add(Convert(subEntry));
            }

            LoadPrototype();
        }

        /// <summary>
        /// Adds the prototype instance to the collection
        /// </summary>
        public void AddPrototype()
        {
            Add(InternalPrototype);
            LoadPrototype();
        }

        /// <summary>
        /// Add a new entry to the collection
        /// </summary>
        public void Add(string type)
        {
            var newEntry = CollectionRoot.GetPrototype(type).Instantiate();
            var converted = Convert(newEntry);
            Add(converted);
        }

        private void Add(ModelAndEntry newItem)
        {
            _internalCollection.Add(newItem);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem.Instance));
        }

        /// <summary>
        /// Remove item from collection
        /// </summary>
        public void Remove(T item)
        {
            for (var index = 0; index < _internalCollection.Count; index++)
            {
                if (_internalCollection[index].Instance != item)
                    continue;

                _internalCollection.RemoveAt(index);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                break;
            }
        }

        /// <inheritdoc />
        protected override List<Entry> GetConfigEntries()
        {
            return _internalCollection.Select(WriteToEntry).ToList();
        }

        /// <summary>
        /// Write instance values to model
        /// </summary>
        private static Entry WriteToEntry(ModelAndEntry item)
        {
            Converter.ToModel(item.Instance, item.Model);
            return item.Model;
        }

        /// <inheritdoc />
        public override IEnumerator<T> GetEnumerator()
        {
            return _internalCollection.Select(e => e.Instance).GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{typeof(T).Name}[{_internalCollection.Count}]";
        }


    }
}
