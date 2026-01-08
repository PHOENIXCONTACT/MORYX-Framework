// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Configuration;

/// <summary>
/// Interface for a value provider filter
/// </summary>
public interface IValueProviderFilter
{
    /// <summary>
    /// Checks the property if it should be set by a value provider
    /// </summary>
    /// <param name="propertyInfo">Property to check</param>
    /// <returns>True if the property should be set otherwise false</returns>
    bool CheckProperty(PropertyInfo propertyInfo);
}