// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Castle.Core;
using Castle.MicroKernel.Registration;

namespace Moryx.Container
{
    /// <summary>
    /// Global runtime container for kernel and module composition
    /// </summary>
    public class GlobalContainer : CastleContainer
    {
        /// <summary>
        /// Initialize castle container to use global registration attributes
        /// </summary>
        public GlobalContainer() 
            : base(new GlobalRegistrator())
        {
            Container.Kernel.ComponentCreated += OnComponentCreated;
        }

        /// <summary>
        /// Event raised when a component of the global container was created
        /// </summary>
        public event EventHandler<object> ComponentCreated;

        private void OnComponentCreated(ComponentModel model, object instance)
        {
            if (ComponentCreated != null)
                ComponentCreated(this, instance);
        }
    }
}
