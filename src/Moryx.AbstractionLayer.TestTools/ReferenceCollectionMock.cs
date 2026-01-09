// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.AbstractionLayer.TestTools;

/// <summary>
/// Dummy implementation of a resource reference collection
/// </summary>
public class ReferenceCollectionMock<T> : List<T>, IReferenceCollection, IReferences<T>
    where T : IResource
{
    /// <inheritdoc />
    public ICollection<IResource> UnderlyingCollection => new List<IResource>((IEnumerable<IResource>)this);

    /// <inheritdoc />
    public event EventHandler<ReferenceCollectionChangedEventArgs> CollectionChanged;
    /// <summary>
    /// Raise the <see cref="CollectionChanged"/> event
    /// </summary>
    /// <param name="eventArgs"></param>
    public void RaiseCollectionChanged(ReferenceCollectionChangedEventArgs eventArgs)
    {
        CollectionChanged?.Invoke(this, eventArgs);
    }
}