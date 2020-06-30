// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Typewrapper factory that is based on the attribute
    /// </summary>
    internal class AttributeWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            return property.GetCustomAttribute<ConfigKeyAttribute>() != null;
        }

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing and writing</param>
        /// <returns>Wrapped property</returns>
        public PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider)
        {
            var att = property.GetCustomAttribute<ConfigKeyAttribute>();
            return new PropertyTypeWrapper(property, formatProvider) { Key = att.Key };
        }

        /// <summary>
        /// Read key from attribute, otherwise null
        /// </summary>
        internal static string FromAttributeOrNull(PropertyInfo property)
        {
            var att = property.GetCustomAttribute<ConfigKeyAttribute>();
            return att?.Key;
        }
    }

    /// <summary>
    /// Decorate property to determine key in config structure
    /// </summary>
    public class ConfigKeyAttribute : Attribute
    {
        /// <summary>
        /// Key in config structure
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Fetch possible value instead of current value
        /// </summary>
        public bool PossibleValues { get; set; }

        /// <summary>
        /// Decorate property to determine key in config structure
        /// </summary>
        /// <param name="key"></param>
        public ConfigKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
