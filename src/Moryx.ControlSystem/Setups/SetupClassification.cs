// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Classification of different types of setups. Those flags might be used outside the kernel
    /// for visualization, prioritization or notifications
    /// </summary>
    [Flags]
    public enum SetupClassification
    {
        /// <summary>
        /// Setup type not specified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Setup is fully automatic
        /// </summary>
        Automatic = 1 << 3,

        /// <summary>
        /// Setup requires manual action
        /// </summary>
        Manual = 1 << 6,

        /// <summary>
        /// The setup confirms a certain state, but not necessarily performs an operation to achieve this
        /// </summary>
        Confirmation = 1 << 9,

        /// <summary>
        /// Material within a container must be changed
        /// </summary>
        MaterialChange = 1 << 12,

        /// <summary>
        /// The setup includes a mechanical reconfiguration that requires tools
        /// </summary>
        RequiresTools = 1 << 15,

        /// <summary>
        /// Part of the setup can only be performed by skilled staff
        /// </summary>
        SkilledWorker = 1 << 18,
    }
}
