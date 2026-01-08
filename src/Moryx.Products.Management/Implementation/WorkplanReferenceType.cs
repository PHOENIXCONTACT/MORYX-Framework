// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    /// <summary>
    /// References between workplan
    /// </summary>
    public enum WorkplanReferenceType
    {
        /// <summary>
        /// Target workplan is a new version of the source workplan
        /// </summary>
        NewVersion = 1000,

        /// <summary>
        /// The workplan is a copy of the other workplan
        /// </summary>
        Copy = 2000,
    }
}
