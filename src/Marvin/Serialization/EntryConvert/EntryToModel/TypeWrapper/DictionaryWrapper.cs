using System;
using System.Reflection;

namespace Marvin.Serialization
{
    internal class DictionaryWrapperFactory : ITypeWrapperFactory
    {
        public bool CanWrap(PropertyInfo property)
        {
            var type = property.PropertyType;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EntryDictionary<>);
        }

        public PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider)
        {
            var key = AttributeWrapperFactory.FromAttributeOrNull(property);
            return new DictionaryWrapper(property, key ?? property.Name, formatProvider) { Required = key != null };
        }
    }

    internal class DictionaryWrapper : CollectionWrapper
    {
        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public DictionaryWrapper(PropertyInfo property, string key, IFormatProvider formatProvider)
            : base(property, key, formatProvider)
        {
        }
    }
}
