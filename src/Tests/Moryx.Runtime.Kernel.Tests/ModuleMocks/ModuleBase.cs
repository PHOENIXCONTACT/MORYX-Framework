// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleBase : IServerModule
    {
        public ServerModuleState State { get; set; }

        /// <inheritdoc />
        public string Name => GetType().Name;

        /// <inheritdoc />
        public INotificationCollection Notifications { get; private set; }

        /// <inheritdoc />
        public IServerModuleConsole Console { get; private set; }

        public IContainer Container => throw new NotImplementedException();

        /// <summary>
        /// Initialize this component and prepare it for incoming taks. This must only involve preparation and must not start
        /// any active functionality and/or periodic execution of logic.
        /// </summary>
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Start all components and modules to begin execution
        /// </summary>
        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop execution, dispose components and return to clean state
        /// </summary>
        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public void AcknowledgeNotification(IModuleNotification notification)
        {
        }

#pragma warning disable 67
        /// <inheritdoc />
        public event EventHandler<ModuleStateChangedEventArgs> StateChanged;
#pragma warning restore 67

    }
}
