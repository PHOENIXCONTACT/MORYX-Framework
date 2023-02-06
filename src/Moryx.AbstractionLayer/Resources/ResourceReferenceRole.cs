// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Role of a resource in a relation decorated with the <see cref="ResourceReferenceAttribute"/>
    /// </summary>
    public enum ResourceReferenceRole
    {
        /// <summary>
        /// The referenced resource is the target entity in the relation.
        /// This is the default reference
        /// </summary>
        Target = 0,
        /// <summary>
        /// The referenced resource is the source entity in the relation
        /// </summary>
        Source = 1
    }
}
