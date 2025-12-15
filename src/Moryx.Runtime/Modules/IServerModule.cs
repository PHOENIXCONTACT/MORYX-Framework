// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Interface for all server modules running within the HeartOfGold application Server.
    /// This interface extends IModule with server specific methods and properties.
    /// Server modules are intended to do the "heavy lifting" within Moryx.
    /// The implement behaviour, business logic and data management of each application.
    /// They might offer web services to connect clients for user interaction.
    /// Each server module has its own lifecylce within the Runtime.
    /// However this life cylce might depend on other server modules.
    /// </summary>
    public interface IServerModule : IModule
    {
        /// <summary>
        /// Internal service provider of the module
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        /// Console to interact with the module
        /// </summary>
        IServerModuleConsole Console { get; }

        /// <summary>
        /// Current state of the server module
        /// </summary>
        ServerModuleState State { get; }

        /// <summary>
        /// Start all components and modules to begin execution
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop execution, dispose components and return to clean state
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Event raised when the current state changes
        /// </summary>
        event EventHandler<ModuleStateChangedEventArgs> StateChanged;
    }

    /// <summary>
    /// Event args for the StateChanged event
    /// </summary>
    public class ModuleStateChangedEventArgs
    {
        /// <summary>
        /// Old state of the module
        /// </summary>
        public ServerModuleState OldState { get; set; }

        /// <summary>
        /// New state of the module
        /// </summary>
        public ServerModuleState NewState { get; set; }
    }
}
