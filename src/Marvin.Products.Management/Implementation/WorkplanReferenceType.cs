using System;

namespace Marvin.Products.Management
{
    /// <summary>
    /// References between workplan
    /// </summary>
    public enum WorkplanReferenceType
    {
        /// <summary>
        /// Target is used as a subworkplan in the source workplan
        /// </summary>
        [Obsolete("Subworkplans are now referenced by steps directly")]
        Subworkplan = 100,

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