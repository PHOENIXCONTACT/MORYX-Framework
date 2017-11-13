using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Host config used to override framework wcf config values
    /// </summary>
    [DataContract]
    public class ExtendedHostConfig : HostConfig
    {
        /// <summary>
        /// Flag if framework values are overriden
        /// </summary>
        [DataMember]
        public bool OverrideFrameworkConfig { get; set; }

        /// <summary>
        /// Port override
        /// </summary>
        [DataMember]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a connection to open before the
        /// transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int OpenTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a connection to close before the
        /// transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int CloseTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a write operation to complete
        /// before the transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int SendTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time that a connection can remain inactive, during
        /// which no application messages are received, before it is dropped.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int ReceiveTimeout { get; set; }
    }
}