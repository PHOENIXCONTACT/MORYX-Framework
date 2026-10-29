// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Target of the <see cref="EntrySerializeAttribute.ConverterType"/>
/// </summary>
public enum EntryConverterTarget
{
    /// <summary>
    /// The member itself. For a collection member the whole collection is converted.
    /// </summary>
    Member,

    /// <summary>
    /// Each item of a collection member.
    /// </summary>
    /// <remarks>For <see cref="Dictionary{TKey, TValue}"/> members only the values are converted, not the keys</remarks>
    Items
}
