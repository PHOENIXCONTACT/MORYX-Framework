using System;
using System.Collections.Generic;
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
        event EventHandler CollectionChanged;
    }
}