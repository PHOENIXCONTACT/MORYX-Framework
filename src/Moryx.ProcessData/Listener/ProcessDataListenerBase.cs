// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;

namespace Moryx.ProcessData.Listener;

/// <summary>
/// Base class for process data listeners with a no custom configuration
/// </summary>
public abstract class ProcessDataListenerBase : ProcessDataListenerBase<ProcessDataListenerConfig>
{
}

/// <summary>
/// Base class for process data listeners with a custom configuration
/// </summary>
public abstract class ProcessDataListenerBase<TConf> : IProcessDataListener, ILoggingComponent
    where TConf : ProcessDataListenerConfig
{
    /// <inheritdoc />
    public IModuleLogger Logger { get; set; }

    /// <summary>
    /// Typed configuration of this listener
    /// </summary>
    public TConf Config { get; private set; }

    /// <inheritdoc />
    public void Initialize(ProcessDataListenerConfig config)
    {
        Config = (TConf)config;
        Logger = Logger.GetChild(Config.PluginName, GetType());
    }

    /// <inheritdoc />
    public virtual void Start()
    {
    }

    /// <inheritdoc />
    public virtual void Stop()
    {
    }

    /// <inheritdoc />
    public void MeasurandAdded(Measurand measurand)
    {
        if (!IsMeasurandEnabled(measurand.Name))
            return;

        OnMeasurandAdded(measurand);
    }

    /// <inheritdoc />
    public void MeasurementAdded(Measurement measurement)
    {
        if (!IsMeasurandEnabled(measurement.Measurand))
            return;

        OnMeasurementAdded(measurement);
    }

    /// <summary>
    /// Will be called if a measurand was added to the process data collector
    /// </summary>
    protected virtual void OnMeasurandAdded(Measurand measurand)
    {
    }

    /// <summary>
    /// Will be called if a measurement was added to the process data collector
    /// </summary>
    protected virtual void OnMeasurementAdded(Measurement measurement)
    {
    }

    private bool IsMeasurandEnabled(string measurandName)
    {
        var config = Config.MeasurandConfigs.FirstOrDefault(m => m.Name == measurandName);
        return config?.IsEnabled ?? false;
    }
}