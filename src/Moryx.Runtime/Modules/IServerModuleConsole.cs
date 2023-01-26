// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Console interface to provide developer interaction with a module
    /// </summary>
    public interface IServerModuleConsole
    {
        /// <summary>
        /// Pass a command to this module
        /// </summary>
        /// <param name="args">Arguments passed to the module</param>
        /// <param name="outputStream">Output stream to be used for feedback</param>
        void ExecuteCommand(string[] args, Action<string> outputStream);
    }
}
