// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.DependentTestModule
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface ISimpleHelloWorldWcfConnectorFactory
    {
        /// <summary>
        /// Create an implementation of the IOrderImporter interface that matches
        /// the given config.
        /// </summary>
        /// <param name="config">Config expected by the OrderImporter implementation</param>
        /// <returns>Fully configured implementation</returns>
        ISimpleHelloWorldWcfConnector Create(SimpleHelloWorldWcfConnectorConfig config);

        /// <summary>
        /// Destroy this importer instance
        /// </summary>
        /// <param name="instance">Importer instance to destroy</param>
        void Destroy(ISimpleHelloWorldWcfConnector instance);
    }
}
