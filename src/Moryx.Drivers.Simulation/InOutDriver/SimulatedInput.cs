// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.Drivers.Simulation.InOutDriver
{
    /// <summary>
    /// Class that represents that inputs and lets the simulated driver modify values and raise events
    /// </summary>
    public class SimulatedInput<TIn> : IInput<TIn>
    {
        /// <summary>
        /// Direkt access to a single value
        /// </summary>
        public TIn Value => Values.ContainsKey(string.Empty) ? Values[string.Empty] : default;

        /// <summary>
        /// Index based access
        /// </summary>
        public TIn this[int index]
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
        public TIn this[string key] => Values.ContainsKey(key) ? Values[key] : default;

        /// <summary>
        /// All access types supported
        /// </summary>
        public SupportedAccess Access => SupportedAccess.Single | SupportedAccess.Index | SupportedAccess.Key | SupportedAccess.Event;

        /// <summary>
        /// Internal value dictionary
        /// </summary>
        public Dictionary<string, TIn> Values { get; } = new Dictionary<string, TIn>();

        /// <summary>
        /// Raise the input changed event for a given key
        /// </summary>
        /// <param name="key"></param>
        public void RaiseInputChanged() => InputChanged?.Invoke(this, new InputChangedEventArgs());

        /// <summary>
        /// Raise the input changed event for a given key
        /// </summary>
        /// <param name="key"></param>
        public void RaiseInputChanged(int index) => InputChanged?.Invoke(this, new InputChangedEventArgs(index));

        /// <summary>
        /// Raise the input changed event for a given key
        /// </summary>
        /// <param name="key"></param>
        public void RaiseInputChanged(string key) => InputChanged?.Invoke(this, new InputChangedEventArgs(key));

        /// <summary>
        /// Event raised, when an input changed
        /// </summary>
        public event EventHandler<InputChangedEventArgs> InputChanged;
    }
}

