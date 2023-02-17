// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Serialization
{
    /// <summary>
    /// Wrapper factory based on the name
    /// </summary>
    internal class NameWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            return true;
        }

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing and writing</param>
        /// <returns>Wrapped property</returns>
        public PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider)
        {
            var wrapper = new PropertyTypeWrapper(property, formatProvider)
            {
                Key = property.Name,
                Required = false
            };
            return wrapper;
        }
    }
}
