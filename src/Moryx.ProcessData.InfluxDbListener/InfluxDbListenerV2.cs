// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
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
    [ExpectedConfig(typeof(InfluxDbListenerConfigV2))]
    [Plugin(LifeCycle.Transient, typeof(IProcessDataListener), Name = nameof(InfluxDbListenerV2))]
    public class InfluxDbListenerV2 : ProcessDataListenerBase<InfluxDbListenerConfigV2>
    {
        /// <summary>
        /// Parallel operations used to start scheduled reports
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        private readonly object _timerLock = new();
        private readonly object _pendingPointsLock = new();
        private readonly ICollection<PointData> _pendingPoints = [];
        private int _timerId;
        private InfluxDBClient _client;
        private short _retries;
        private CancellationTokenSource _cancellationTokenSource;

        #region Lifecycle

        /// <inheritdoc/>
        public override void Start()
        {
            var influxOptions = CreateClientConfiguration();
            _client = new InfluxDBClient(influxOptions);
            _cancellationTokenSource = new CancellationTokenSource();
            _retries = Config.MaxNumberOfRetries;
            _timerId = 0;
            base.Start();
        }

        private InfluxDBClientOptions CreateClientConfiguration()
        {
            var protocol = Config.UseTls ? "https" : "http";

            var influxOptions = new InfluxDBClientOptions($"{protocol}://{Config.Host}:{Config.Port}")
            {
                Org = Config.Organisation,
                Bucket = Config.DatabaseName,
                VerifySsl = Config.VerifySslCertificate
            };

            // These options can't be assigned in the property initializer block
            // Because the influx library throws when null or empty strings are assigned.
            if (!string.IsNullOrEmpty(Config.Token?.Trim()))
            {
                influxOptions.Token = Config.Token;
            }
            else
            {
                influxOptions.Username = Config.Username;
                influxOptions.Password = Config.Password;
            }

            if (!string.IsNullOrEmpty(Config.Proxy?.Trim()))
            {
                influxOptions.WebProxy = new WebProxy(Config.Proxy)
                {
                    BypassProxyOnLocal = Config.BypassProxyOnLocalHost
                };
            }

            return influxOptions;
        }

        /// <inheritdoc/>
        public override void Stop()
        {
            if (_timerId != 0)
            {
                ParallelOperations.StopExecution(_timerId);
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _client?.Dispose();
            base.Stop();
        }

        #endregion

        /// <inheritdoc />
        protected override void OnMeasurandAdded(Measurand measurand)
        {
            lock (_pendingPointsLock)
                _pendingPoints.AddRange(measurand.Measurements.Select(GetPointFromMeasurement));

            StartTimerIfStopped();
        }

        /// <inheritdoc />
        protected override void OnMeasurementAdded(Measurement measurement)
        {
            lock (_pendingPointsLock)
                _pendingPoints.Add(GetPointFromMeasurement(measurement));
            
            StartTimerIfStopped();
        }

        private void StartTimerIfStopped()
        {

            lock (_timerLock)
                if (_timerId == 0)
                    // Using .Wait() is ugly and I hate it, but as of now ParallelOperations cannot handle async actions. Change as soon as possible
                    _timerId = ParallelOperations.ScheduleExecution(() => ReportAsync(_cancellationTokenSource.Token).Wait(), Config.ReportIntervalMs, Config.ReportIntervalMs);
        }

        private async Task ReportAsync(CancellationToken cancellationToken)
        {
            PointData[] points;
            lock (_pendingPointsLock)
            {
                points = [.. _pendingPoints];
                _pendingPoints.Clear();
            }

            if (points.Length > 0)
                await UploadAsync(points, cancellationToken);
        }

        private async Task UploadAsync(PointData[] points, CancellationToken cancellationToken)
        {
#if COMMERCIAL
            if (!LicenseCheck.HasLicense())
                return;
#endif

            try
            {
                // ToDo: When the license check has a caching mechanism, use WriteApi and remove custom buffering and retry mechanism
                var writeApi = _client.GetWriteApiAsync();
                Logger.LogDebug("Writing {number} points to influxDB '{name}'", points.Length, Config.DatabaseName);
                await writeApi.WritePointsAsyncWithIRestResponse(points, cancellationToken: cancellationToken);
                _retries = Config.MaxNumberOfRetries;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while writing points to influxDB '{name}'. Moving {number} points back to queue. " +
                    "Remaining retries: {retries}", Config.DatabaseName, points.Length, --_retries);
                
                lock (_pendingPointsLock)
                    _pendingPoints.AddRange(points);

                if (_retries < 0)
                {
                    ParallelOperations.StopExecution(_timerId);
                    throw new Exception($"Out of retires for {Config.ListenerName}. Stopping listener...", e);
                }                
            }
        }

        private PointData GetPointFromMeasurement(Measurement measurement)
        {
            try
            {
                var builder = PointData.Builder.Measurement(measurement.Measurand);
                foreach (var tag in measurement.Tags)
                    builder.Tag(tag.Name, tag.Value);
                foreach (var field in measurement.Fields)
                    AddDataToPoint(builder, field.Name, field.Value);
                builder.Tag("application", Platform.Current.ProductName);
                builder.Timestamp(measurement.TimeStamp.ToUniversalTime(), WritePrecision.Ms);

                return builder.ToPointData();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while transforming measurement: {0}.", measurement);
                throw;
            }
        }

        private void AddDataToPoint(PointData.Builder point, string prefix, object value)
        {
            switch (value)
            {
                case null:
                    break;
                case bool b:
                    point.Field(prefix, b);
                    break;
                case int i:
                    point.Field(prefix, i);
                    break;
                case uint u:
                    point.Field(prefix, u);
                    break;
                case float f:
                    point.Field(prefix, f);
                    break;
                case double d:
                    point.Field(prefix, d);
                    break;
                case decimal d:
                    point.Field(prefix, (double)d);
                    break;
                case long l:
                    point.Field(prefix, l);
                    break;
                case ulong ul:
                    point.Field(prefix, ul);
                    break;
                case byte b:
                    point.Field(prefix, b);
                    break;
                case short s:
                    point.Field(prefix, s);
                    break;
                case ushort u:
                    point.Field(prefix, u);
                    break;
                case DateTime dt:
                    point.Field(prefix, dt.Ticks);
                    break;
                case TimeOnly timeOnly:
                    point.Field(prefix, timeOnly.Ticks);
                    break;
                case DateOnly dateOnly:
                    point.Field(prefix, dateOnly.ToString("yyyy-MM-dd"));
                    break;
                case TimeSpan ts:
                    point.Field(prefix, ts.TotalMilliseconds);
                    break;
                case string s:
                    point.Field(prefix, s);
                    break;
                case Guid guid:
                    point.Field(prefix, guid.ToString());
                    break;
                case IDictionary dictionary:
                    foreach (var key in dictionary.Keys)
                    {
                        AddDataToPoint(point, $"{prefix}_{key}", dictionary[key]);
                    }
                    break;
                case IEnumerable enumerable:
                    {
                        int i = 0;
                        foreach (var elem in enumerable)
                        {
                            AddDataToPoint(point, $"{prefix}_{i}", elem);
                            i++;
                        }
                        point.Field($"{prefix}_Length", i);
                    }
                    break;
                default:
                    var type = value.GetType();
                    if (type.IsClass)
                    {
                        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        foreach (var prop in props)
                        {
                            AddDataToPoint(point, $"{prefix}_{prop.Name}", prop.GetValue(value));
                        }
                    }
                    else
                    {
                        Logger?.LogWarning("No case matched {typeName}", type.Name);
                        point.Field(prefix, value);
                    }
                    break;
            }
        }
    }
}

