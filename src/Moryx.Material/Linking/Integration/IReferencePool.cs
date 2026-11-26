// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Linking.Integration;

/// <summary>
/// Pool of managed references kept in sync with the business objects of an integration.
/// </summary>
/// <typeparam name="TReference">Type of the managed references.</typeparam>
public interface IReferencePool<TReference>
    where TReference : Reference
{
    /// <summary>
    /// Registers <paramref name="reference"/> as being used by a link and returns the managed reference
    /// matching it. Adds the reference to the pool if it is not tracked yet.
    /// </summary>
    Task<TReference> AcquireAsync(TReference reference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a previously acquired reference.
    /// </summary>
    void Release(TReference? reference);
}
