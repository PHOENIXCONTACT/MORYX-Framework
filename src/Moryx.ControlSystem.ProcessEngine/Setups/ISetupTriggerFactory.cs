// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.ProcessEngine.Setups
{
    /// <summary>
    /// Factory to instantiate <see cref="ISetupTrigger"/> implementations
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface ISetupTriggerFactory
    {
        /// <summary>
        /// Create trigger based on config
        /// </summary>
        ISetupTrigger Create(SetupTriggerConfig config);

        /// <summary>
        /// Destroy a setup trigger instance
        /// </summary>
        void Destroy(ISetupTrigger instance);
    }
}
