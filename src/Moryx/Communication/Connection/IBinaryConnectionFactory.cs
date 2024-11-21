// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Communication
{
    /// <summary>
    /// Factory to create connections based on config and header interpreter
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IBinaryConnectionFactory
    {
        /// <summary>
        /// Create a connection instance
        /// </summary>
        /// <param name="config">Config of the <see cref="IBinaryConnection"/> implementation</param>
        /// <param name="validator">Interpreter for validation of incoming header bytes</param>
        IBinaryConnection Create(BinaryConnectionConfig config, IMessageValidator validator);

        /// <summary>
        /// Destroy the no longer required instance
        /// </summary>
        void Destroy(IBinaryConnection instance);
    }
}
