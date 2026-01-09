// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData;

/// <summary>
/// Tags holding additional information related to a <see cref="DataField"/>
/// </summary>
public class DataTag
{
    /// <summary>
    /// Name of this tag
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Value of this tag
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="DataTag"/>.
    /// </summary>
    public DataTag(string name, string value)
    {
        Name = name;
        Value = value;
    }
}