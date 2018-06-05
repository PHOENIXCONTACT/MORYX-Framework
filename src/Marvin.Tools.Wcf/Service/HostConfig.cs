using System.ComponentModel;
using System.Runtime.Serialization;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Configuration class used to provide typed wcf config
    /// </summary>
    [DataContract]
    public class HostConfig
    {
        /// <summary>
        /// Endpoint of host
        /// </summary>
        [DataMember]
        [DefaultValue("NewService")]
        public string Endpoint { get; set; }

        /// <summary>
        /// Desired service binding
        /// </summary>
        [DataMember]
        public ServiceBindingType BindingType { get; set; }

        /// <summary>
        /// Flag if service requires Authentification
        /// </summary>
        [DataMember]
        public bool RequiresAuthentification { get; set; }

        /// <summary>
        /// Metadata page enabled
        /// </summary>
        [DataMember]
        public bool MetadataEnabled { get; set; }

        /// <summary>
        /// Help page enabled
        /// </summary>
        [DataMember]
        public bool HelpEnabled { get; set; }
    }
}
