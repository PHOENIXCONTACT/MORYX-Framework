// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Identity;

/// <summary>
/// Null object implementation for identity, representing a non-identifiable object.
/// </summary>
public class NullIdentity : IIdentity
{
    private static NullIdentity _instance;

    /// <summary>
    /// Singleton instance indicating non-identifiability
    /// </summary>
    public static NullIdentity Instance => _instance ??= new NullIdentity();

    /// <summary>
    /// Private constructor to enforce singleton
    /// </summary>
    private NullIdentity()
    {
    }

    /// <inheritdoc/>
    public string Identifier => string.Empty;

    /// <inheritdoc/>
    public bool Equals(IIdentity other) => other is NullIdentity;

    /// <inheritdoc/>
    public void SetIdentifier(string identifier) =>
        throw new InvalidOperationException($"Cannot set identifier on {nameof(NullIdentity)}");
}