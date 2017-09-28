using Marvin.Configuration;

namespace Marvin.Runtime.Kernel.Tests
{
    /// <summary>
    /// Test subconfig
    /// </summary>
    public class RuntimeConfigManagerTestConfig2 : IConfig
    {
        /// <summary>
        /// Current state of the config object.
        /// </summary>
        public ConfigState ConfigState
        {
            get; set; 
        }

        /// <summary>
        /// Exception message if load failed.
        /// </summary>
        public string LoadError
        {
            get; set;
        }
    }
}
