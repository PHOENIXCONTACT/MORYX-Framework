// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
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
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            ValidateHealthState();
            ProcessDataCollector.AddMeasurement(measurement);
        }

        public void Add(Measurand measurand)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif
            ValidateHealthState();
            ProcessDataCollector.AddMeasurand(measurand);
        }
    }
}
