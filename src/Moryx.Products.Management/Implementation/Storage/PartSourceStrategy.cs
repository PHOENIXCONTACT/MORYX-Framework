// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    /// <summary>
    /// Different types of instance creation
    /// </summary>
    public enum PartSourceStrategy
    {
        /// <summary>
        /// Default instance creation from partlink
        /// </summary>
        FromPartlink = 0, // TODO: Rename to FromPartLink in the next major

        /// <summary>
        /// Create parts from entity relation
        /// </summary>
        FromEntities = 10,
    }
}
