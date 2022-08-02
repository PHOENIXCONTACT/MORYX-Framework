// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx
{
    /// <summary>
    /// Interface for application executables like HeartOfGold or HeartOfLead
    /// </summary>
    public interface IApplicationRuntime
    {
        /// <summary>
        /// Top Level container
        /// </summary>
        IContainer GlobalContainer { get; }
    }

    /// <summary>
    /// Additional interface to interact with the MORYX application
    /// </summary>
    public interface IApplicationRuntimeExecution : IApplicationRuntime
    {
        /// <summary>
        /// Load environment and composition
        /// </summary>
        int Load();

        /// <summary>
        /// Execute the
        /// </summary>
        int Execute();
    }
}