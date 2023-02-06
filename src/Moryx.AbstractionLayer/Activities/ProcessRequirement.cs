// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// This enum is used, to define whether the resource must report the process, if the process is optional
    /// or must not be present at all
    /// </summary>
    public enum ProcessRequirement
    {
        /// <summary>
        /// The activity requires that the resource is in possesion of the process. This is the default
        /// for must activities that are performed during the execution of the process.
        /// </summary>
        Required,
        /// <summary>
        /// Posession of the process is not necessary, but allowed. This is usually the case for virtual
        /// activities like preparing print images or fetching serial numbers.
        /// </summary>
        NotRequired,
        /// <summary>
        /// The activity only works on empty resources. A process must not be present. This is the case when starting
        /// a process by inserting a part into the machine or confirming the execution of a process.
        /// </summary>
        Empty
    }
}
