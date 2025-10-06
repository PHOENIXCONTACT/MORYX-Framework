// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Container;
using Moryx.ProcessData.Listener;
using Moryx.Tools;

namespace Moryx.ProcessData.Monitor
{
    [Plugin(LifeCycle.Singleton, typeof(IProcessDataCollector))]
    internal class ProcessDataCollector : IProcessDataCollector
    {
        private readonly ICollection<IProcessDataListener> _listeners = new List<IProcessDataListener>();

        #region Dependencies

        /// <summary>
        /// Factory to create new <see cref="IProcessDataListener"/>
        /// </summary>
        public IProcessDataListenerFactory ListenerFactory { get; set; }

        /// <summary>
        /// Config manager of the runtime to edit the config on the fly
        /// </summary>
        public IConfigManager RuntimeConfigManager { get; set; }

        /// <summary>
        /// Configuration of the module for listeners
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public void Start()
        {
            foreach (var listenerConfig in ModuleConfig.Listeners)
            {
                var listener = ListenerFactory.Create(listenerConfig);
                listener.Start();
                _listeners.Add(listener);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            foreach (var listener in _listeners.ToArray())
            {
                listener.Stop();
                _listeners.Remove(listener);
                ListenerFactory.Destroy(listener);
            }
        }

        /// <inheritdoc />
        public void AddMeasurand(Measurand measurand)
        {
            ValidateListeners(measurand.Name);

            _listeners.ForEach(l => l.MeasurandAdded(measurand));

            MeasurandAdded?.Invoke(this, measurand);
        }

        /// <inheritdoc />
        public void AddMeasurement(Measurement measurement)
        {
            ValidateListeners(measurement.Measurand);

            _listeners.ForEach(l => l.MeasurementAdded(measurement));

            MeasurementAdded?.Invoke(this, measurement);
        }

        private readonly object _configSaveLock = new();

        private void ValidateListeners(string measurandName)
        {
            var modified = false;
            foreach (var listenerConfig in ModuleConfig.Listeners)
            {
                var measurandConfig = listenerConfig.MeasurandConfigs.FirstOrDefault(m => m.Name == measurandName);
                if (measurandConfig != null)
                    continue;

                listenerConfig.MeasurandConfigs.Add(new MeasurandConfig
                {
                    Name = measurandName,
                    IsEnabled = false
                });
                modified = true;
            }

            if (!modified)
                return;

            lock (_configSaveLock)
                RuntimeConfigManager.SaveConfiguration(ModuleConfig);
        }

        /// <inheritdoc />
        public event EventHandler<Measurand> MeasurandAdded;

        /// <inheritdoc />
        public event EventHandler<Measurement> MeasurementAdded;
    }
}
