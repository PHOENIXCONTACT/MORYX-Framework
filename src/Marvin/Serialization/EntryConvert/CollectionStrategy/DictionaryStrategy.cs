// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Marvin.Serialization
{
    /// <summary>
    /// Dictionary implementation of <see cref="ICollectionStrategy"/>
    /// </summary>
    internal class DictionaryStrategy : ICollectionStrategy
    {
        private readonly IDictionary _dictionary;
        private readonly IList _toDelete = new List<object>();
        private readonly ICustomSerialization _serialization;

        public DictionaryStrategy(IDictionary dictionary, ICustomSerialization serialization)
        {
            _serialization = serialization;
            _dictionary = dictionary;
        }

        public IEnumerable<Entry> Serialize()
        {
            var entries = new List<Entry>();
            foreach (var key in _dictionary.Keys)
            {
                var value = _dictionary[key];
                var keyString = key.ToString();

                var subEntry = EntryConvert.EncodeObject(value, _serialization);
                subEntry.Name = keyString;
                subEntry.Identifier = keyString;

                entries.Add(subEntry);
            }
            return entries;
        }

        public IEnumerable<string> Keys()
        {
            return _dictionary.Keys.OfType<object>().Select(key => key.ToString()).ToArray();
        }

        public object ElementAt(string key)
        {
            var keyValue = EntryValue(key);
            return _dictionary[keyValue];
        }

        public void Added(Entry entry, object addedValue)
        {
            var keyValue = EntryValue(entry.Name);
            _dictionary.Add(keyValue, addedValue);
        }

        public void Updated(Entry entry, object updatedValue)
        {
            Removed(entry.Identifier);
            Added(entry, updatedValue);
        }

        public void Removed(string key)
        {
            var entryValue = EntryValue(key);
            _dictionary.Remove(entryValue);
        }

        public void Flush()
        {
        }

        private object EntryValue(string entry)
        {
            var entryType = _dictionary.GetType().GenericTypeArguments[0];
            var entryValue = EntryConvert.ToObject(entryType, entry, _serialization.FormatProvider);
            return entryValue;
        }
    }
}
