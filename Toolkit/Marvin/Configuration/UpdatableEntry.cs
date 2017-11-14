using System;
using System.Runtime.Serialization;

namespace Marvin.Configuration
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
