// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Interface for validators that ensure properties have been filled as expected
    /// </summary>
    public interface IValueProviderValidator
    {
        /// <summary>
        /// Checks the property if it was correctly set by a value provider
        /// </summary>
        /// <param name="propertyInfo">Property to check</param>
        /// <param name="target">Object the property is on. Can be used to access the value for instance</param>
        /// <returns>True if the property was correctly set</returns>
        bool CheckProperty(PropertyInfo propertyInfo, object target);
    }
}