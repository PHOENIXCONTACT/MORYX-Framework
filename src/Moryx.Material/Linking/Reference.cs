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
    public ReferenceState State { get; protected set; } = ReferenceState.Initialized;
}

public static class ReferenceExtensions
{
    extension(Reference reference)
    {
        public bool IsActive() => reference.State == ReferenceState.Active;
        public bool IsValid() => reference.State != ReferenceState.Unavailable;
    }
}
