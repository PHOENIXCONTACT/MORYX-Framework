// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Logging
{
    /// <summary>
    /// Represents a generic target logger.
    /// Can be implemented to provide any type of logging framework.
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// Log the message with the given <see cref="LogLevel"/> and additional exception and information
        /// </summary>
        void Log(ILogMessage logMessage);
    }
}
