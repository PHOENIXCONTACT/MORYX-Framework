// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.ProcessData.Monitor
{
    /// <summary>
    /// Collector component where all measurements are summarized
    /// </summary>
    internal interface IProcessDataCollector : IPlugin
    {
        /// <summary>
        /// Adds a measurand to the process data collector
        /// </summary>
        void AddMeasurand(Measurand measurand);

        /// <summary>
        /// Adds a measurement to the process data collector
        /// </summary>
        void AddMeasurement(Measurement measurement);

        /// <summary>
        /// Will be raised if a measurand was added
        /// </summary>
        event EventHandler<Measurand> MeasurandAdded;

        /// <summary>
        /// Will be raised if a measurement was added
        /// </summary>
        event EventHandler<Measurement> MeasurementAdded;
    }
}
