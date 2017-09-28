using System;
using System.Runtime.Serialization;
using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance.Wcf
{
    /// <summary>
    /// Configurates log messages
    /// </summary>
    [DataContract]
    public class LogMessages
    {
        /// <summary>
        /// Id of the appender where the messages belongs to.
        /// </summary>
        [DataMember]
        public int AppenderId { get; set; }

        /// <summary>
        /// True when the appender id belongs to a valid appender.
        /// </summary>
        [DataMember]
        public bool Successful { get; set; } 

        /// <summary>
        /// List of log messages.
        /// </summary>
        [DataMember]
        public LogMessageModel[] Messages { get; set; }
    }

    /// <summary>
    /// Model which describes a log message.
    /// </summary>
    [DataContract]
    public class LogMessageModel
    {
        /// <summary>
        /// Instance of a logger.
        /// </summary>
        [DataMember]
        public PluginLoggerModel Logger { get; set; }
        /// <summary>
        /// Name of the class which created the logger message.
        /// </summary>
        [DataMember]
        public string ClassName { get; set; }
        /// <summary>
        /// The severity of this message.
        /// </summary>
        [DataMember]
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// The log message iteself.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// Time when the log message occured.
        /// </summary>
        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}
