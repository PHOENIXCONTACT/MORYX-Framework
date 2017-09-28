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
                var entry = new Entry
                {
                    Key = new EntryKey
                    {
                        Name = keyString,
                        Identifier = keyString
                    }
                };

                var type = value.GetType();
                if (type.IsValueType)
                {
                    entry.Value = new EntryValue
                    {
                        Type = EntryConvert.TransformType(type),
                        Current = value.ToString()
                    };
                }
                else
                {
                    entry.Value = new EntryValue
                    {
                        Type = EntryValueType.Class,
                        Current = value.GetType().Name
                    };
                }

                // Recursive call for the children
                if (entry.Value.Type == EntryValueType.Class)
                    entry.SubEntries.AddRange(EntryConvert.EncodeObject(value, _serialization));

                entries.Add(entry);
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
            var keyvalue = EntryConvert.ToObject(keyType, key);
            return keyvalue;
        }
    }
}