// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Logging
{
    /// <summary>
    /// Factory for <see cref="IModuleLogger"/>
    /// </summary>
    public interface IModuleLoggerFactory
    {
        /// <summary>
        /// Create a named logger instance
        /// </summary>
        IModuleLogger Create(string name);
    }
}
