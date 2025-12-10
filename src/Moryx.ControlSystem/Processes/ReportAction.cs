// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Actions to report a running process
    /// </summary>
    public enum ReportAction
    {
        /// <summary>
        /// The process is broken or damaged
        /// </summary>
        Broken,
        /// <summary>
        /// The process was physically removed
        /// </summary>
        Removed
    }
}
