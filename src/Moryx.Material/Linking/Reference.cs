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

    /// <summary>
    /// Checks whether <paramref name="other"/> identifies the same business object as this reference,
    /// independent of the reference instance and its <see cref="State"/>.
    /// </summary>
    public abstract bool Matches(Reference? other);
}

public static class ReferenceExtensions
{
    extension(Reference reference)
    {
        public bool IsActive() => reference.State == ReferenceState.Active;
        public bool IsValid() => reference.State != ReferenceState.Unavailable;
    }

    extension(Reference? reference)
    {
        /// <summary>
        /// Value-based equality check, also handling <c>null</c> on both sides.
        /// </summary>
        public bool ValueEquals(Reference? other) => reference is null ? other is null : reference.Matches(other);
    }
}
