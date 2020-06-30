// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Logging
{
    /// <summary>
    /// Represens a generic target logger. 
    /// Can be implemented to provide any type of logging framework.
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// Simply log the message wit the given <see cref="LogLevel"/>
        /// </summary>
        void Log(LogLevel loglevel, string message);

        /// <summary>
        /// Log the message with the given <see cref="LogLevel"/> and aditional exception
        /// </summary>
        void Log(LogLevel loglevel, string message, Exception exception);
    }
}
