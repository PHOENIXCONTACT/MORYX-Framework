// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.ProcessData.Listener;

namespace Moryx.ProcessData.Monitor
{
    /// <summary>
    /// Factory to create process data listeners from configuration
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IProcessDataListenerFactory
    {
        /// <summary>
        /// Creates a process data listener from the given configuration
        /// </summary>
        IProcessDataListener Create(ProcessDataListenerConfig config);

        /// <summary>
        /// Destroys the given process data listener
        /// </summary>
        /// <param name="listener"></param>
        void Destroy(IProcessDataListener listener);
    }
}
