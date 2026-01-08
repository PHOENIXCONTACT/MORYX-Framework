// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;

namespace Moryx.Serialization
{
    /// <summary>
    /// List implementation of <see cref="ICollectionStrategy"/>
    /// </summary>
    internal class ListStrategy : ICollectionStrategy
    {
        private readonly IList _list;
        private readonly IList _toDelete = new List<object>();
        private readonly ICustomSerialization _customSerialization;

        public ListStrategy(IList list, ICustomSerialization customSerialization)
        {
            _list = list;
            _customSerialization = customSerialization;
        }

        public IEnumerable<Entry> Serialize()
        {
            var entries = new List<Entry>();
            for (int index = 0; index < _list.Count; index++)
            {
                var entry = CollectionStrategyTools.CreateSub(_list[index], index, _customSerialization);
                entries.Add(entry);
            }
            return entries;
        }

        public IEnumerable<string> Keys()
        {
            return CollectionStrategyTools.GenerateKeys(_list.Count);
        }

        public object ElementAt(string key)
        {
            return _list[int.Parse(key)];
        }

        public void Added(Entry entry, object addedValue)
        {
            _list.Add(addedValue);
        }

        public void Updated(Entry entry, object updatedValue)
        {
            _list[int.Parse(entry.Identifier)] = updatedValue;
        }

        public void Removed(string key)
        {
            _toDelete.Add(_list[int.Parse(key)]);
        }

        public void Flush()
        {
            foreach (var missing in _toDelete)
            {
                _list.Remove(missing);
            }
        }
    }
}
