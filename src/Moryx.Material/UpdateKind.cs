// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material;

/// <summary>
/// Flags that describe which parts of a material container's content are updated.
/// </summary>
[Flags]
public enum UpdateKind
{
    /// <summary>
    /// No material property is changed.
    /// </summary>
    NoOperation = 0,

    /// <summary>
    /// The material reference is changed.
    /// </summary>
    MaterialType = 1,

    /// <summary>
    /// The filling level or quantity is changed.
    /// </summary>
    FillingLevel = 1 << 1,

    /// <summary>
    /// The change denotes a relative rather than an absolute change
    /// </summary>
    Relative =  1 << 8,
}
