// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Material.Linking;

/// <summary>
/// Base class for wrappers around domain object references owned by integrations.
/// </summary>
/// <remarks>
/// References are owned by the integration facade (not the resource). Properties on
/// subclasses are mapped from the underlying business object once it has been resolved.
/// </remarks>
[DataContract]
public abstract class Reference
{
    /// <summary>
    /// Current state of this reference.
    /// </summary>
    [DataMember]
    public ReferenceState State { get; protected internal set; } = ReferenceState.Initialized;
}

/// <summary>
/// State machine of <see cref="Reference"/>.
/// </summary>
public enum ReferenceState
{
    /// <summary>Reference info available; business object not yet resolved.</summary>
    Initialized = 0,

    /// <summary>Business object has been resolved; mapped properties accessible.</summary>
    Active = 1,

    /// <summary>Business object intentionally detached (e.g. shutdown).</summary>
    Inactive = 2,

    /// <summary>Lookup for the business object failed.</summary>
    Unavailable = 3
}