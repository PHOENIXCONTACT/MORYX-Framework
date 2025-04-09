// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    /// <summary>
    /// Classification of a connector to increase semantic meaning
    /// Flags: Failure || Workplan borders || Exit point ||  Entry point
    /// </summary>
    [Flags]
    public enum NodeClassification
    {
        /// <summary>
        /// Intermediate step
        /// </summary>
        Intermediate = 0,

        /// <summary>
        /// Connector is an entry point
        /// </summary>
        Entry = 1,

        /// <summary>
        /// Connector is an exit point
        /// </summary>
        Exit = 2,

        /// <summary>
        /// Outer limits of a workplan
        /// </summary>
        WorkplanBorder = 4,

        /// <summary>
        /// Connector only reached when error occurred
        /// </summary>
        Failure = 8,

        /// <summary>
        /// Start point for a normal workplan
        /// </summary>
        Start = Entry | WorkplanBorder,

        /// <summary>
        /// End point for a normal workplan
        /// </summary>
        End = Exit | WorkplanBorder,

        /// <summary>
        /// Reset and abort point for a normal workplan if an unrecoverable failure occurred.
        /// </summary>
        Failed = Exit | Failure
    }
}
