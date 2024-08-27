// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Result of a value provider
    /// </summary>
    public enum ValueProviderResult
    {
        /// <summary>
        /// Value was set
        /// </summary>
        Handled,
        /// <summary>
        /// Value was skipped
        /// </summary>
        Skipped
    }

    /// <summary>
    /// ValueProvider interface
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// Provides value to a property
        /// </summary>
        /// <param name="parent">Instance of property which shall be set</param>
        /// <param name="property">Property to set</param>
        /// <returns></returns>
        ValueProviderResult Handle(object parent, PropertyInfo property);
    }
}
