// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.TestModule
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IHelloWorldWcfConnectorFactory
    {
        /// <summary>
        /// Create an implementation of the IOrderImporter interface that matches
        /// the given config.
        /// </summary>
        /// <param name="config">Config expected by the OrderImporter implementation</param>
        /// <returns>Fully configured implementation</returns>
        IHelloWorldWcfConnector Create(HelloWorldWcfConnectorConfig config);

        /// <summary>
        /// Destroy this importer instance
        /// </summary>
        /// <param name="instance">Importer instance to destroy</param>
        void Destroy(IHelloWorldWcfConnector instance);
    }
}
