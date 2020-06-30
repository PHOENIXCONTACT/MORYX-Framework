// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Serialization
{
    /// <summary>
    /// Interface for all strategies that can read values from 
    /// </summary>
    internal interface ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        bool CanWrap(PropertyInfo property);

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <param name="formatProvider"><see cref="IFormatProvider"/> used for parsing and writing</param>
        /// <returns>Wrapped property</returns>
        PropertyTypeWrapper Wrap(PropertyInfo property, IFormatProvider formatProvider);
    }
}
