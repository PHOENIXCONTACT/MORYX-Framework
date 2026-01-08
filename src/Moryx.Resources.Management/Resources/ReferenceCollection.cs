// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management;

/// <summary>
/// Collection that wraps another collection of references
/// </summary>
internal class ReferenceCollection<TResource> : IReferences<TResource>
    where TResource : class, IResource
{
    private readonly Resource _parent;

    private readonly PropertyInfo _property;

    /// <summary>
    /// Create a new reference collection
    /// </summary>
    public ReferenceCollection(Resource parent, PropertyInfo property, ICollection<IResource> underlyingCollection)
    {
        _parent = parent;
        _property = property;
        UnderlyingCollection = underlyingCollection;
    }

    /// <summary>
    /// Forward count to underlying collection
    /// </summary>
    public int Count => UnderlyingCollection.Count;

    /// <summary>
    /// Reference collections are never read only
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// The underlying collection wrapped by this <see cref="ReferenceCollection{TResource}"/>
    /// </summary>
    public ICollection<IResource> UnderlyingCollection { get; }

    /// <summary>
    /// Add a resource the the reference collection
    /// </summary>
    public void Add(TResource item)
    {
        lock (UnderlyingCollection)
            UnderlyingCollection.Add(item);
        RaiseCollectionChanged();
    }

    /// <summary>
    /// Remove a resource reference
    /// </summary>
    public bool Remove(TResource item)
    {
        var result = false;
        lock (UnderlyingCollection)
            result = UnderlyingCollection.Remove(item);

        if (result)
            RaiseCollectionChanged();
        return result;
    }

    /// <summary>
    /// Clear all references in the collection
    /// </summary>
    public void Clear()
    {
        lock (UnderlyingCollection)
            UnderlyingCollection.Clear();
        RaiseCollectionChanged();
    }

    /// <summary>
    /// Check if the collection contains this item
    /// </summary>
    public bool Contains(TResource item)
    {
        lock (UnderlyingCollection)
            return UnderlyingCollection.Contains(item);
    }

    /// <summary>
    /// Copy the entire collection to another array
    /// </summary>
    public void CopyTo(TResource[] array, int arrayIndex)
    {
        lock (UnderlyingCollection)
            UnderlyingCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Iterate over the collection
    /// </summary>
    public IEnumerator<TResource> GetEnumerator()
    {
        lock (UnderlyingCollection)
            return UnderlyingCollection.Cast<TResource>().ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void RaiseCollectionChanged()
    {
        var args = new ReferenceCollectionChangedEventArgs(_parent, _property);
        CollectionChanged?.Invoke(this, args);
    }
    public event EventHandler<ReferenceCollectionChangedEventArgs> CollectionChanged;
}