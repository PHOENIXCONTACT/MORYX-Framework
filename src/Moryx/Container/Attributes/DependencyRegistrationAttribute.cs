// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Container
{
    /// <summary>
    /// Enum defining the installer mode for dependency registration
    /// </summary>
    public enum InstallerMode
    {
        /// <summary>
        /// Register all components from this assembly
        /// </summary>
        All,
        /// <summary>
        /// Register components for specified services only
        /// </summary>
        Specified
    }

    /// <summary>
    /// Use this attribute to enable auto installer for the subcomponent assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DependencyRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Executes DependencyAutoInstaller in this assembly
        /// </summary>
        public DependencyRegistrationAttribute(InstallerMode mode)
        {
            InstallerMode = mode;
            RequiredTypes = new Type[0];
        }

        /// <summary>
        /// Executes selective DependencyAutioInstaller in this assembly
        /// </summary>
        /// <param name="minimalService">Minimum of one service to avoid empty constructor</param>
        /// <param name="services">Services to install</param>
        public DependencyRegistrationAttribute(Type minimalService, params Type[] services)
        {
            InstallerMode = InstallerMode.Specified;
            RequiredTypes = new Type[services.Length + 1];
            RequiredTypes[0] = minimalService;
            services.CopyTo(RequiredTypes, 1);
        }

        /// <summary>
        /// Mode used by the installer for dependency registration
        /// </summary>
        public InstallerMode InstallerMode { get; private set; }

        /// <summary>
        /// Types that need to be registered from this assembly
        /// </summary>
        public Type[] RequiredTypes { get; private set; }

        /// <summary>
        /// Optional initializer to be executed
        /// </summary>
        public Type Initializer { get; set; }
    }

    /// <summary>
    /// Interface for components that execute further initialization
    /// </summary>
    public interface ISubInitializer
    {
        /// <summary>
        /// Execute further initialization
        /// </summary>
        void Initialize(IContainer container);
    }
}
