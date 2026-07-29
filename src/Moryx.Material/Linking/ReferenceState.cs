// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking;

/// <summary>
/// State machine of <see cref="Reference"/>.
/// </summary>
public enum ReferenceState
{
    /// <summary>Reference info available; business object not yet resolved.</summary>
    Initialized = 0,

    /// <summary>Business object has been resolved; mapped properties accessible.</summary>
    Active = 1,

    /// <summary>Business object intentionally detached (e.g. shutdown or operation completed).</summary>
    Inactive = 2,

    /// <summary>Lookup for the business object failed.</summary>
    Unavailable = 3
}
