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
        public int PortOverride { get; set; }
    }
}