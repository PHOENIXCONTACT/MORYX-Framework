// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ProcessData.Listener
{
    /// <summary>
    /// Interface for process data listeners
    /// </summary>
    public interface IProcessDataListener : IConfiguredPlugin<ProcessDataListenerConfig>
    {
        /// <summary>
        /// Will be raised if a measurand was added
        /// </summary>
        void MeasurandAdded(Measurand measurand);

        /// <summary>
        /// Will be raised if a measurement was added
        /// </summary>
        void MeasurementAdded(Measurement measurement);
    }
}