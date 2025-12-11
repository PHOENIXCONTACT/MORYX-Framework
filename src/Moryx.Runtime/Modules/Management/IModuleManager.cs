// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Kernel component responsible for module lifecycle access and dependencies
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Start all modules in cascading order
        /// </summary>
        Task StartModulesAsync();

        /// <summary>
        /// Stop all modules in cascading order
        /// </summary>
        Task StopModulesAsync();

        /// <summary>
        /// Initialize a server module
        /// </summary>
        /// <param name="module"></param>
        Task InitializeModuleAsync(IServerModule module);

        /// <summary>
        /// Start a specific module and all its dependencies
        /// </summary>
        /// <param name="module">Module to start</param>
        Task StartModuleAsync(IServerModule module);

        /// <summary>
        /// Stop a specific modules
        /// </summary>
        /// <param name="module">Module to stop</param>
        Task StopModuleAsync(IServerModule module);

        /// <summary>
        /// Restart the module and all of its dependecies
        /// </summary>
        /// <param name="module"></param>
        Task ReincarnateModuleAsync(IServerModule module);

        /// <summary>
        /// All modules managed by the service manager
        /// </summary>
        IEnumerable<IServerModule> AllModules { get; }

        /// <summary>
        /// Event raised when one of the modules changed its state
        /// </summary>
        event EventHandler<ModuleStateChangedEventArgs> ModuleStateChanged;

        /// <summary>
        /// All other modules this module depends on
        /// </summary>
        /// <param name="module">Service to check</param>
        /// <returns>Types of all dependencies</returns>
        IEnumerable<IServerModule> StartDependencies(IServerModule module);

        /// <summary>
        /// Get the full dependency tree
        /// </summary>
        /// <returns></returns>
        IModuleDependencyTree DependencyTree { get; }

        /// <summary>
        /// Get or set a services behaviour using 
        /// </summary>
        /// <typeparam name="T">Type of behaviour</typeparam>
        IBehaviourAccess<T> BehaviourAccess<T>(IServerModule module);
    }

    /// <summary>
    /// Interface to provide access to module behaviour
    /// </summary>
    /// <typeparam name="T">Type of behavior</typeparam>
    public interface IBehaviourAccess<T>
    {
        /// <summary>
        /// Get or set the modules behaviour
        /// </summary>
        T Behaviour { get; set; }
    }
}
