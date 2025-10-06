// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.ProcessData.Listener;
using Moryx.Threading;
using Microsoft.Extensions.Logging;

namespace Moryx.ProcessData.SpreadsheetsListener
{
    /// <summary>
    /// ProcessDataListener which is writing measurements to spreadsheets
    /// </summary>
    [ExpectedConfig(typeof(SpreadsheetsListenerConfig))]
    [Plugin(LifeCycle.Transient, typeof(IProcessDataListener), Name = nameof(SpreadsheetsListener))]
    public class SpreadsheetsListener : ProcessDataListenerBase<SpreadsheetsListenerConfig>
    {
        /// <summary>
        /// Parallel operations used to start scheduled reports
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }
        private readonly Dictionary<string, CsvStructure> _pendingCsvMeasurements = new();
        private int _timerId;

        /// <inheritdoc/>
        public override void Stop()
        {
            lock (_pendingCsvMeasurements)
            {
                if (_timerId != 0)
                    ParallelOperations.StopExecution(_timerId);
                foreach (var csvStructure in _pendingCsvMeasurements.Values)
                {
                    csvStructure.WriteRows().Wait();
                    csvStructure.Close();
                }
                _pendingCsvMeasurements.Clear();
            }
        }

        /// <inheritdoc />
        protected override void OnMeasurandAdded(Measurand measurand)
        {
            lock (_pendingCsvMeasurements)
            {
                foreach (var measurement in measurand.Measurements)
                    AddPendingMeasurement(measurement).Wait();
            }
            CheckOrStartTimer();
        }

        /// <inheritdoc />
        protected override void OnMeasurementAdded(Measurement measurement)
        {
            lock (_pendingCsvMeasurements)
            {
                AddPendingMeasurement(measurement).Wait();
            }
            CheckOrStartTimer();
        }

        private void CheckOrStartTimer()
        {
            if (_timerId == 0)
                _timerId = ParallelOperations.ScheduleExecution(Report, Config.ReportIntervalMs, -1);
        }


        private void Report()
        {
            _timerId = 0;
            lock (_pendingCsvMeasurements)
            {
                //_csvHelper.CreateCsvFiles();
                Task.Run(async delegate { await WriteCsvFiles(); });
            }
        }

        private async Task AddPendingMeasurement(Measurement measurement)
        {
            CsvStructure csvMeasurements;
            if (!_pendingCsvMeasurements.TryGetValue(measurement.Measurand, out csvMeasurements))
            {
                csvMeasurements = new CsvStructure(Config, measurement.Measurand, Logger);
                _pendingCsvMeasurements.Add(measurement.Measurand, csvMeasurements);
            }
            await csvMeasurements.AddMeasurement(measurement);
        }

        private async Task WriteCsvFiles()
        {
            if (_pendingCsvMeasurements.Count == 0)
            {
                Logger.LogDebug("No records to upload");
                return;
            }
            foreach (var csvStructure in _pendingCsvMeasurements.Values)
            {
                await csvStructure.WriteRows();
            }
        }
    }
}

