using System;
using System.Collections.Generic;
using System.Reflection;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Interface for <see cref="ReferenceCollection{TResource}"/>
    /// </summary>
    internal interface IReferenceCollection
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

    internal class ReferenceCollectionChangedEventArgs : EventArgs
    {
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
}