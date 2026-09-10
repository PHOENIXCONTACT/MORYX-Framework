// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Serialization;

/// <summary>
/// Interface for custom struct serialization to Entry sub-entries
/// </summary>
internal interface IStructSerializer
{
    /// <summary>
    /// The type this serializer handles
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Encode a struct value into an Entry with sub-entries
    /// </summary>
    Entry Encode(object value, IFormatProvider formatProvider);

    /// <summary>
    /// Decode an Entry with sub-entries back into a struct value
    /// </summary>
    object Decode(Entry entry, IFormatProvider formatProvider);
}
