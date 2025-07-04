// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Additional interface for the activity dispatcher to access the reported sessions
    /// </summary>
    internal interface IActivityDispatcher
    {
        /// <summary>
        /// Export sessions
        /// </summary>
        ResourceAndSession[] ExportSessions();
    }
}
