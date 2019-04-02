using System;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Factory that creates <see cref="CollectionWrapper"/> instances
    /// </summary>
    internal class CollectionWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            var type = property.PropertyType;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EntryCollection<>);
        }

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing and writing</param>
        /// <returns>Wrapped property</returns>
        public PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider)
        {
            var key = AttributeWrapperFactory.FromAttributeOrNull(property);
            return new CollectionWrapper(property, key ?? property.Name, formatProvider) { Required = key != null };
        }
    }

    /// <summary>
    /// Property wrapper for collection properties
    /// </summary>
    internal class CollectionWrapper : PropertyTypeWrapper
    {
        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public CollectionWrapper(PropertyInfo property, string key, IFormatProvider formatProvider) : base(property, formatProvider)
        {
            Key = key;
        }

        /// <summary>
        /// Read value from config
        /// </summary>
        protected override object ReadFromConfig(Entry entry)
        {
            return Activator.CreateInstance(Property.PropertyType, entry);
        }

        /// <summary>
        /// Read value from property and write to config
        /// </summary>
        public override void ReadValue(object source, Entry target)
        {
            var collection = (IEntryCollection)Property.GetValue(source);
            target.SubEntries = collection.ConfigEntries();
        }
    }
}
