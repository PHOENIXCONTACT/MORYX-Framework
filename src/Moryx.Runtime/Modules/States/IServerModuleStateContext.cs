// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using Moryx.StateMachines;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Interface for server module base to access state based transitions
    /// </summary>
    internal interface IServerModuleStateContext : IStateContext
    {
        /// <summary>
        /// Initialize the module
        /// </summary>
        void Initialize();

        /// <summary>
        /// Destructs an initialized module
        /// </summary>
        void Destruct();

        /// <summary>
        /// Start the module
        /// </summary>
        void Start();

        /// <summary>
        /// Called when module was started
        /// </summary>
        void Started();

        /// <summary>
        /// Stop the module
        /// </summary>
        void Stop();

        /// <summary>
        /// Report a failure
        /// </summary>
        void ReportError(Exception exception);

        /// <summary>
        /// Adds a notification to the module
        /// </summary>>
        void LogNotification(IModuleNotification notification);
    }
}
