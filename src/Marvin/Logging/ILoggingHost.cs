// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Logging
{
    /// <summary>
    /// Interface for all components hosting a top level logger
    /// </summary>
    public interface ILoggingHost
    {
        /// <summary>
        /// Name of this host. Used for logger name structure
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Logger instance
        /// </summary>
        IModuleLogger Logger { get; set; }
    }
}
