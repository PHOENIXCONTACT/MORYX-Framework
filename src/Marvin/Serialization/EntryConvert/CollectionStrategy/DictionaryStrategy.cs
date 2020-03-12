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
                subEntry.Key.Name = keyString;
                subEntry.Key.Identifier = keyString;

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
            var keyValue = KeyValue(key);
            return _dictionary[keyValue];
        }

        public void Added(EntryKey key, object addedValue)
        {
            var keyValue = KeyValue(key.Name);
            _dictionary.Add(keyValue, addedValue);
        }

        public void Updated(EntryKey key, object updatedValue)
        {
            Removed(key.Identifier);
            Added(key, updatedValue);
        }

        public void Removed(string key)
        {
            var keyValue = KeyValue(key);
            _dictionary.Remove(keyValue);
        }

        public void Flush()
        {
        }

        private object KeyValue(string key)
        {
            var keyType = _dictionary.GetType().GenericTypeArguments[0];
            var keyvalue = EntryConvert.ToObject(keyType, key, _serialization.FormatProvider);
            return keyvalue;
        }
    }
}
