using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Moryx.Serialization
{
    public class ArrayIListStrategy : ICollectionStrategy
    {
        private readonly IList _list;
        private readonly IList _toDelete = new List<object>();
        private readonly ICustomSerialization _customSerialization;
        private readonly ICustomAttributeProvider _property;
        private readonly object _instance;
        private readonly IList _toAdd = new List<object>();

        public ArrayIListStrategy(IList list, ICustomSerialization customSerialization,
             ICustomAttributeProvider attributeProvider, object instance)
        {
            _list = list;
            _customSerialization = customSerialization;
            _property = attributeProvider;
            _instance = instance;
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
            _toAdd.Add(addedValue);
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
           
            if(_property is PropertyInfo propertyInfo)
            {
                var type = propertyInfo.PropertyType;
                var elementType = type.GenericTypeArguments[0];
                var list = Array.CreateInstance(elementType,_list.Count - _toDelete.Count + _toAdd.Count);
                var index = 0;
                foreach (var e in _list)
                {
                    if (!_toDelete.Contains(e))
                    {
                        list.SetValue(e, index++);
                    }                       
                }
                foreach(var e in _toAdd)
                {
                    list.SetValue(e, index++);
                }
                   
                propertyInfo.SetValue(_instance, list);
            }
        }
    }
}
