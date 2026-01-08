// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.SetupProvider
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
