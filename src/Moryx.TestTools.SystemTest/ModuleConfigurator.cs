// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Maintenance.Plugins.Modules;

namespace Moryx.TestTools.SystemTest
{
    /// <summary>
    /// Helper to modify the configuration ServerModule
    /// </summary>
    public class ModuleConfigurator
    {
        private readonly HeartOfGoldController _hogController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfigurator"/> class.
        /// </summary>
        /// <param name="hogController">The hog controller.</param>
        public ModuleConfigurator(HeartOfGoldController hogController)
        {
            _hogController = hogController;
        }

        /// <summary>
        /// Gets the "trunk" ServerModule configuration.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>the "trunk" configuration</returns>
        public Config GetServiceConfig(string serviceName)
        {
            return _hogController.GetConfig(serviceName);
        }
    }
}
