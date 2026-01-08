// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Different options for execution timing of a trigger
    /// </summary>
    public enum SetupExecution
    {
        /// <summary>
        /// Trigger is relevant before job execution
        /// </summary>
        BeforeProduction,

        /// <summary>
        /// Trigger is relevant after execution
        /// </summary>
        AfterProduction
    }
}
