// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Logging
{
    /// <summary>
    /// This interface allows framework components to access a module logger and log entries in their name
    /// </summary>
    public interface ILoggingComponent
    {
        /// <summary>
        /// Logger of this component
        /// </summary>
        IModuleLogger Logger { get; set; }
    }
}
