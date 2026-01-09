// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.Drivers.Simulation.InOutDriver;

/// <summary>
/// Class that represents that inputs and lets the simulated driver modify values and raise events
/// </summary>
public class SimulatedInput : IInput
{
    /// <summary>
    /// Direkt access to a single value
    /// </summary>
    public object Value => Values.ContainsKey(string.Empty) ? Values[string.Empty] : default;

    /// <summary>
    /// Index based access
    /// </summary>
    public object this[int index]
    {
        get
        {
            var key = index.ToString("D");
            return Values.ContainsKey(key) ? Values[key] : default;
        }
    }

    /// <summary>
    /// Key based access to the inputs
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[string key] => Values.ContainsKey(key) ? Values[key] : default;

    /// <summary>
    /// All access types supported
    /// </summary>
    public SupportedAccess Access => SupportedAccess.Single | SupportedAccess.Index | SupportedAccess.Key | SupportedAccess.Event;

    /// <summary>
    /// Internal value dictionary
    /// </summary>
    public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

    /// <summary>
    /// Raise the input changed event for a given key
    /// </summary>
    public void RaiseInputChanged(object value) => InputChanged?.Invoke(this, new InputChangedEventArgs { Value = value});

    /// <summary>
    /// Raise the input changed event for a given key
    /// </summary>
    public void RaiseInputChanged(int index, object value) => InputChanged?.Invoke(this, new InputChangedEventArgs(index, value));

    /// <summary>
    /// Raise the input changed event for a given key
    /// </summary>
    public void RaiseInputChanged(string key, object value) => InputChanged?.Invoke(this, new InputChangedEventArgs(key, value));

    /// <summary>
    /// Event raised, when an input changed
    /// </summary>
    public event EventHandler<InputChangedEventArgs> InputChanged;
}