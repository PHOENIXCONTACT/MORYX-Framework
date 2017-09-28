using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Global part configuration class to simplify port change
    /// </summary>
    [DataContract]
    public class WcfConfig : IConfig
    {
        /// <summary>
        /// Host for wcf services
        /// </summary>
        [DataMember]
        [DefaultValue("localhost")]
        public string Host { get; set; }

        /// <summary>
        /// Port used for http bindings
        /// </summary>
        [DataMember]
        [DefaultValue(80)]
        public int HttpPort { get; set; }

        /// <summary>
        /// Port used for net tcp bindings
        /// </summary>
        [DataMember]
        [DefaultValue(816)]
        public int NetTcpPort { get; set; }

        /// <summary>
        /// Current state of the config object. This should be decorated with the data member in order to save
        ///             the valid state after finalized configuration.
        /// </summary>
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <summary>
        /// Exception message if load failed. This must not be decorated with a data member attribute.
        /// </summary>
        public string LoadError { get; set; }
    }
}
