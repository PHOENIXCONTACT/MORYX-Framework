// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.Drivers.Simulation.InOutDriver
{
    public class SimulatedOutput : IOutput
    {
        /// <summary>
        /// Set the single output
        /// </summary>
        public object Value
        {
            get
            {
                var key = string.Empty;
                return Values.ContainsKey(key) ? Values[key] : default;
            }
            set
            {
                var key = string.Empty;
                Values[key] = value;
                OutputSet?.Invoke(this, key);
            }
        }

        /// <summary>
        /// Set output based on index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                var key = index.ToString("D");
                return Values.ContainsKey(key) ? Values[key] : default;
            }
            set
            {
                var key = index.ToString("D");
                Values[key] = value;
                OutputSet?.Invoke(this, key);
            }
        }

        /// <summary>
        /// Key based output
        /// </summary>
        public object this[string key]
        {
            get => Values.ContainsKey(key) ? Values[key] : default;
            set
            {
                Values[key] = value;
                OutputSet?.Invoke(this, key);
            }
        }

        /// <summary>
        /// Internal dictionary holding the values
        /// </summary>
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Simulation supports all access modes
        /// </summary>
        public SupportedAccess Access => SupportedAccess.Single | SupportedAccess.Index | SupportedAccess.Key;

        /// <summary>
        /// Event raised, when an output was set externally
        /// </summary>
        public event EventHandler<string> OutputSet;
    }
}

