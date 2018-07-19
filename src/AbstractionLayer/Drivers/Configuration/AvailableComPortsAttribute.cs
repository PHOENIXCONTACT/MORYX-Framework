using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Microsoft.Win32;

namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Attribute listing COM ports on maintenance
    /// </summary>
    public class AvailableComPortsAttribute : PossibleValuesAttribute
    {
        /// <summary>
        /// Resolves the registered COM ports.
        /// </summary>
        public override IEnumerable<string> GetValues(IContainer pluginContainer)
        {
            var registryKey = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");

            if (registryKey == null)
                return Enumerable.Range(1, 9).Select(p => $"COM{p}");

            var names = registryKey.GetValueNames();
            return names.Select(name => (string)registryKey.GetValue(name)).ToArray();
        }

        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public override bool OverridesConversion => false;

        /// <summary>
        /// Flag if new values shall be updated from the old value
        /// </summary>
        public override bool UpdateFromPredecessor => false;
    }
}