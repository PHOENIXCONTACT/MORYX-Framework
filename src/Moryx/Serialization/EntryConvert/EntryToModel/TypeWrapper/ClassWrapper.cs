// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Serialization
{
    /// <summary>
    /// Factory to create class property wrappers
    /// </summary>
    internal class ClassWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            var type = property.PropertyType;
            return type.IsClass && type != typeof(string) && type != typeof(string[]);
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
            return new ClassWrapper(property, key ?? property.Name, formatProvider) { Required = key != null };
        }
    }

    /// <summary>
    /// Property type wrapper for entries of type class
    /// </summary>
    internal class ClassWrapper : PropertyTypeWrapper
    {
        private readonly EntryToModelConverter _converter;

        /// <summary>
        /// Create match wrapper for a property
        /// </summary>
        public ClassWrapper(PropertyInfo property, string key, IFormatProvider formatProvider)
            : base(property, formatProvider)
        {
            Key = key;
            _converter = EntryToModelConverter.Create(property.PropertyType, formatProvider);
        }

        /// <summary>
        /// Read value from config
        /// </summary>
        protected override object ReadFromConfig(Entry entry)
        {
            var instance = Activator.CreateInstance(Property.PropertyType);
            _converter.FromModel(entry, instance);
            return instance;
        }

        /// <summary>
        /// Read value from property and write to config
        /// </summary>
        public override void ReadValue(object source, Entry target)
        {
            var value = Property.GetValue(source);
            _converter.ToModel(value, target);
        }
    }
}
