// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Activities
{
    /// <summary>
    /// Type of modification performed by an <see cref="IInstanceModificationActivity"/>
    /// </summary>
    public enum InstanceModificationType
    {
        /// <summary>
        /// Article was not modified
        /// </summary>
        None,
        /// <summary>
        /// Instance was modified and needs to be saved
        /// </summary>
        Changed,
        /// <summary>
        /// An instance was created and must be saved
        /// </summary>
        Created,
        /// <summary>
        /// An instance was loaded and must be read from the database
        /// </summary>
        Loaded
    }
}
