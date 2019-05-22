using System;
using Marvin.Modules;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleBase : IServerModule
    {
        public ServerModuleState State { get; set; }

        /// <summary>
        /// Unique name for this module within the platform it is designed for
        /// </summary>
        public string Name => GetType().Name;

        /// <summary>
        /// Notifications published by this module
        /// </summary>
        public INotificationCollection Notifications { get; private set; }

        /// <summary>
        /// Console to interact with the module
        /// </summary>
        public IServerModuleConsole Console { get; private set; }

        /// <summary>
        /// Initialize this component and prepare it for incoming taks. This must only involve preparation and must not start
        ///             any active functionality and/or periodic execution of logic.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Start all components and modules to begin execution
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Stop execution, dispose components and return to clean state
        /// </summary>
        public void Stop()
        {
        }

#pragma warning disable 67
        /// <inheritdoc />
        public event EventHandler<ModuleStateChangedEventArgs> StateChanged;
#pragma warning restore 67

    }
}