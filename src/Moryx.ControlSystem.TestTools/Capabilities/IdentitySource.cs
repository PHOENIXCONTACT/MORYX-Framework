// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Capabilities
{
    /// <summary>
    /// Source of the identity
    /// </summary>
    [Flags]
    public enum IdentitySource
    {
        /// <summary>
        /// Should assign a identity from a pool
        /// </summary>
        Pool = 1,

        /// <summary>
        /// Identity should be assigned manual
        /// </summary>
        Manual = 1 << 3,

        /// <summary>
        /// Identity should be assigned automatically
        /// </summary>
        Automatic = 1 << 6,

        /// <summary>
        /// Identity should be assigned by a camera (also automatically)
        /// </summary>
        Camera = 1 << 9,

        /// <summary>
        /// Identity should be assigned by a camera (also manually)
        /// </summary>
        Scanner = 1 << 12,
    }
}
