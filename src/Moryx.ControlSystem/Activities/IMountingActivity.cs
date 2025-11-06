// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Activities
{
    /// <summary>
    /// Mounting operation performed when this activity is completed
    /// </summary>
    public enum MountOperation
    {
        /// <summary>
        /// Previous state remains
        /// </summary>
        Unchanged = 0,

        /// <summary>
        /// Mount process on carrier
        /// </summary>
        Mount = 1,

        /// <summary>
        /// Remove process from carrier
        /// </summary>
        Unmount = 2
    }

    /// <summary>
    /// Special interface to identify activities that perform mount operations
    /// </summary>
    public interface IMountingActivity : IActivity
    {
        /// <summary>
        /// Operation this activity performs
        /// </summary>
        MountOperation Operation { get; }
    }
}
