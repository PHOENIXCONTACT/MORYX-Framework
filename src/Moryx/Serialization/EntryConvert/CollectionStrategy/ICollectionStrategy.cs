// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Serialization
{
    /// <summary>
    /// Interface to decouple collection operations from the differences between 
    /// Arrays and lists
    /// </summary>
    internal interface ICollectionStrategy
    {
        /// <summary>
        /// Serialize the collection to entries
        /// </summary>
        IEnumerable<Entry> Serialize();

        /// <summary>
        /// Current set of element identifiers
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Keys();
        
        /// <summary>
        /// Retrieve element at a certain position
        /// </summary>
        object ElementAt(string key);

        /// <summary>
        /// Called when a new value was added
        /// </summary>
        void Added(Entry entry, object addedValue);

        /// <summary>
        /// Method called when a value was updated
        /// </summary>
        void Updated(Entry entry, object updatedValue);

        /// <summary>
        /// Remove a value from the collection that is missing in the entry collection
        /// </summary>
        void Removed(string key);

        /// <summary>
        /// Write pending changes to the target collection
        /// </summary>
        void Flush();
    }
}
