// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Different request types for ReadyToWork
    /// </summary>
    public enum ReadyToWorkType
    {
        /// <summary>
        /// Define a special default to avoid default=Push
        /// </summary>
        Unset,
        /// <summary>
        /// The push
        /// </summary>
        Push,
        /// <summary>
        /// The pull
        /// </summary>
        Pull
    }
}
