// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.Drivers.Simulation.InOutDriver
{
    public class SimulatedOutput<TOut> : IOutput<TOut>
    {
        /// <summary>
        /// Set the single output
        /// </summary>
        public TOut Value
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
        public TOut this[int index]
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
        public TOut this[string key]
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
        public Dictionary<string, TOut> Values { get; } = new Dictionary<string, TOut>();

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

