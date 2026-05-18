// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Addition interface for <see cref="IReferences{TResource}"/>
/// </summary>
public interface IReferenceCollection
{
    /// <summary>
    /// The wrapped collection
    /// </summary>
    ICollection<IResource> UnderlyingCollection { get; }

    /// <summary>
    /// Event raised when the collection was changed
    /// </summary>
    event EventHandler<ReferenceCollectionChangedEventArgs> CollectionChanged;
}

// TODO: Merge with IReferenceCollection in next major
// TODO: Add a way to synchronize access to the underlying collection properly.
//       Is not a best practice and not possible in an async context
/// <summary>
/// Temporary extension interface to include new functionality that should be merged into IReferenceCollection later
/// </summary>
public interface IReferenceCollectionExtended : IReferenceCollection
{
    /// <summary>
    /// Notifies the collection that the UnderlyingCollection was changed directly
    /// </summary>
    void UnderlyingCollectionChanged();
}

/// <summary>
/// Event args for the <see cref="IReferenceCollection.CollectionChanged"/> event
/// </summary>
public class ReferenceCollectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Create a new instance of the event args
    /// </summary>
    public ReferenceCollectionChangedEventArgs(Resource parent, PropertyInfo collectionProperty)
    {
        Parent = parent;
        CollectionProperty = collectionProperty;
    }

    /// <summary>
    /// Resource instance this collection belongs to
    /// </summary>
    public Resource Parent { get; }

    /// <summary>
    /// Property represented by this collection
    /// </summary>
    public PropertyInfo CollectionProperty { get; }
}
