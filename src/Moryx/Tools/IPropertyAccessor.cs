// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.Tools;

/// <summary>
/// Interface for dynamic access to properties
/// </summary>
public interface IPropertyAccessor<in TBase, TValue>
{
    /// <summary>
    /// Name of the wrapped property
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Wrapped property
    /// </summary>
    PropertyInfo Property { get; }

    /// <summary>
    /// Read the property on the given instance
    /// </summary>
    TValue ReadProperty(TBase instance);

    /// <summary>
    /// Write the property on the instance
    /// </summary>
    void WriteProperty(TBase instance, TValue value);
}