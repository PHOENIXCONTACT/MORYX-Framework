// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Moryx.Configuration
{
    /// <summary>
    /// Base class for all configuration entries that support live update
    /// </summary>
    [DataContract]
    public class UpdatableEntry : IUpdatableConfig
    {
        /// <summary>
        /// Event raised when the config was modified by external code
        /// </summary>
        public event EventHandler<ConfigChangedEventArgs> ConfigChanged;
        /// <summary>
        /// Explicit interface to hide raise method
        /// </summary>
        /// <param name="modifiedProperties">Names of properties that where modified</param>
        void IUpdatableConfig.RaiseConfigChanged(params string[] modifiedProperties)
        {
            ConfigChanged?.Invoke(this, new ConfigChangedEventArgs(modifiedProperties));
        }
    }
}
