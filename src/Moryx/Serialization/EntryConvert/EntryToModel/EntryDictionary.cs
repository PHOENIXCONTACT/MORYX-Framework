// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Marvin.Serialization
{
    /// <summary>
    /// Collection type for typed representations of config collections
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection</typeparam>
    public class EntryDictionary<T> : EntryCollectionBase<T>
        where T : class, new()
    {
        /// <summary>
        /// Internally we wrap an observable collection
        /// </summary>
        private readonly Dictionary<string, ModelAndEntry> _internalCollection = new Dictionary<string, ModelAndEntry>();

        /// <summary>
        /// Create a new instance of the collection
        /// </summary>
        /// <param name="collectionRoot"></param>
        public EntryDictionary(Entry collectionRoot) : base(collectionRoot)
        {
            foreach (var subEntry in collectionRoot.SubEntries)
            {
                _internalCollection.Add(subEntry.Identifier, Convert(subEntry));
            }

            LoadPrototype();
        }

        /// <summary>
        /// Add a new entry to the collection
        /// </summary>
        public void Add(string key, string type)
        {
            var newEntry = CollectionRoot.GetPrototype(type).Instantiate();
            var converted = Convert(newEntry);
            Add(key, converted);
        }

        private void Add(string key, ModelAndEntry newItem)
        {
            _internalCollection.Add(key, newItem);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem.Instance));
        }

        /// <summary>
        /// Remove item from collection
        /// </summary>
        public void Remove(string key)
        {
            var item = _internalCollection.FirstOrDefault(k => k.Key == key);
            if (_internalCollection.Remove(key))
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item.Value.Instance));
            }
        }

        /// <summary>
        /// Returns an instance by the given key
        /// </summary>
        /// <param name="key"></param>
        public T this[string key]
        {
            get { return _internalCollection[key].Instance; }
            set { _internalCollection[key].Instance = value; }
        }

        /// <inheritdoc />
        protected override List<Entry> GetConfigEntries()
        {
            return _internalCollection.Select(WriteToEntry).ToList();
        }

        /// <summary>
        /// Write instance values to model
        /// </summary>
        private static Entry WriteToEntry(KeyValuePair<string, ModelAndEntry> item)
        {
            Converter.ToModel(item.Value.Instance, item.Value.Model);
            return item.Value.Model;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<T> GetEnumerator()
        {
            return _internalCollection.Select(e => e.Value.Instance).GetEnumerator();
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
