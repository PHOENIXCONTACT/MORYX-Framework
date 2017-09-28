using System.Runtime.Serialization;
using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance.Wcf
{
    /// <summary>
    /// Model which represens a plugin logger. 
    /// </summary>
    [DataContract]
    public class PluginLoggerModel
    {
        /// <summary>
        /// Name of this logger.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The level for which this logger is configured. See <see cref="LogLevel"/> for level information.
        /// </summary>
        [DataMember]
        public LogLevel ActiveLevel { get; set; }

        /// <summary>
        /// List of childs of this logger.
        /// </summary>
        [DataMember]
        public PluginLoggerModel[] ChildLogger { get; set; }

        /// <summary>
        /// The parent of this logger. 
        /// </summary>
        [DataMember]
        public PluginLoggerModel Parent { get; set; }
    }
}
