// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Modules;
using Moryx.ProcessData.Listener;
using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.ProcessData.InfluxDbListener
{
    /// <summary>
    /// ProcessDataListener which is writing measurements to an influx database
    /// </summary>
    [ExpectedConfig(typeof(InfluxDbListenerConfig))]
    [Plugin(LifeCycle.Transient, typeof(IProcessDataListener), Name = nameof(InfluxDbListener))]
    public class InfluxDbListener : ProcessDataListenerBase<InfluxDbListenerConfig>
    {
        /// <summary>
        /// Parallel operations used to start scheduled reports
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        private readonly ICollection<LineProtocolPoint> _pendingPoints = new List<LineProtocolPoint>();
        private int _timerId;

        /// <inheritdoc />
        protected override void OnMeasurandAdded(Measurand measurand)
        {
            lock (_pendingPoints)
                _pendingPoints.AddRange(measurand.Measurements.Select(GetPointFromMeasurement));

            CheckOrStartTimer();
        }

        /// <inheritdoc />
        protected override void OnMeasurementAdded(Measurement measurement)
        {
            lock (_pendingPoints)
                _pendingPoints.Add(GetPointFromMeasurement(measurement));

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
            LineProtocolPoint[] points;
            lock (_pendingPoints)
            {
                points = _pendingPoints.ToArray();
                _pendingPoints.Clear();
            }

            Task.Run(async delegate { await Upload(points); });
        }

        private async Task Upload(LineProtocolPoint[] points)
        {
            var payload = new LineProtocolPayload();

            foreach (var point in points)
                payload.Add(point);

            var client = new LineProtocolClient(new Uri($"http://{Config.Host}:{Config.Port}"), Config.DatabaseName, Config.Username, Config.Password);
            try
            {
                Logger.Log(LogLevel.Debug, "Writing {0} points to influxDB '{1}'", points.Length, Config.DatabaseName);

                var influxResult = await client.WriteAsync(payload);
                if (!influxResult.Success)
                {
                    Logger.Log(LogLevel.Warning, "Error while writing points to influxDb. Moving points back to queue: {0}", influxResult.ErrorMessage);
                    AddPointsBackToTheList();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while writing points to influx db. Moving points back to queue.");
                AddPointsBackToTheList();
            }

            void AddPointsBackToTheList()
            {
                lock (_pendingPoints)
                    _pendingPoints.AddRange(points);
            }
        }

        private LineProtocolPoint GetPointFromMeasurement(Measurement measurement)
        {
            try
            {
                var fields = measurement.Fields.ToDictionary(s => s.Name, s => s.Value);
                var tags = measurement.Tags.ToDictionary(s => s.Name, s => s.Value);
                tags.Add("application", Platform.Current.ProductName);

                var point = new LineProtocolPoint(measurement.Measurand, fields, tags,
                    measurement.TimeStamp.ToUniversalTime());

                return point;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while transforming measurement: {0}.", measurement);
                throw;
            }
        }
    }
}

