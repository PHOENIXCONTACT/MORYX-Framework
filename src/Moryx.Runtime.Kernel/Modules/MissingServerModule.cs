// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moryx.Runtime.Kernel.Modules
{
    /// <summary>
    /// Class representing a missing module in the application
    /// </summary>
    internal class MissingServerModule : IServerModule
    {
        public IServerModuleConsole Console => null;

        public ServerModuleState State => ServerModuleState.Missing;

        public string Name => GetName();

        public INotificationCollection Notifications =>  null;

        public event EventHandler<ModuleStateChangedEventArgs> StateChanged;

        public MissingServerModule(Type service, bool optional)
        {
            RepresentedService = service;
            Optional = optional;
        }

        /// <summary>
        /// Gets the name of the Module
        /// </summary>
        /// <returns></returns>
        private string GetName()
        {        
            var isInterfaceAndHasIasFirstCharacter = RepresentedService.Name.ElementAt(0).ToString().ToLower() == "i" &&
                RepresentedService.IsInterface;

            if (isInterfaceAndHasIasFirstCharacter)
                return RepresentedService.Name.Remove(0, 1); // remove "i" in the interface name
            else return RepresentedService.Name;
        }

        public void AcknowledgeNotification(IModuleNotification notification)
        {
        }

        public void Initialize()
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public Type RepresentedService { get; private set; }

        public IContainer Container => throw new NotImplementedException();

        public bool Optional { get; }
    }
}

