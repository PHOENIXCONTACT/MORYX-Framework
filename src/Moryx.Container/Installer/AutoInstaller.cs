// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Container
{
    /// <summary>
    /// Installer for automatic registration
    /// </summary>
    public class AutoInstaller : IContainerInstaller
    {
        private readonly Assembly _targetAssembly;

        /// <summary>
        /// Create a new instance of the <see cref="AutoInstaller"/> for this assembly 
        /// </summary>
        public AutoInstaller(Assembly targetAssembly)
        {
            _targetAssembly = targetAssembly ?? GetType().Assembly;
        }


        /// <summary>
        /// Install components to the container
        /// </summary>
        /// <param name="registrator">Registrator to register new types</param>
        public virtual void Install(IComponentRegistrator registrator)
        {
            // Install all components
            foreach (var type in _targetAssembly.GetTypes())
            {
                // Register all we want
                if (ShallInstall(registrator, type))
                    registrator.Register(type);
            }
        }

        /// <summary>
        /// Method to determine if this component shall be installed
        /// </summary>
        protected internal virtual bool ShallInstall(IComponentRegistrator registrator, Type foundType)
        {
            return registrator.ShallInstall(foundType);
        }
    }
}
