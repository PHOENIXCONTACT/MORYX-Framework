// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Monitor
{
    internal class ProcessDataMonitorFacade : IProcessDataMonitor, IFacadeControl
    {
        public Action ValidateHealthState { get; set; }

        public IProcessDataCollector ProcessDataCollector { get; set; }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void Add(Measurement measurement)
        {
            ValidateHealthState();
            ProcessDataCollector.AddMeasurement(measurement);
        }

        public void Add(Measurand measurand)
        {
            ValidateHealthState();
            ProcessDataCollector.AddMeasurand(measurand);
        }
    }
}
