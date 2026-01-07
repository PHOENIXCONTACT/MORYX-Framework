// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model.Attributes;

/// <summary>
/// Attribute to mark properties as part of the connection string with the given key
/// </summary>
public class ConnectionStringKeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringKeyAttribute"/> class.
    /// </summary>
    /// <param name="key">Key in the connection string</param>
    public ConnectionStringKeyAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Key in the connection string
    /// </summary>
    public string Key { get; }
}
