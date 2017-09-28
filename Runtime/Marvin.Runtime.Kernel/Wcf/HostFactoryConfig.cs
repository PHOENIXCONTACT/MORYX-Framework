using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Runtime.Kernel.Wcf
{
    /// <summary>
    /// Configuration for the host factory.
    /// </summary>
    [DataContract]
    public class HostFactoryConfig : ConfigBase
    {
        /// <summary>
        /// Configuration to enable/disable the version service.
        /// </summary>
        [DataMember]
        public bool VersionServiceDisabled { get; set; }
    }
}
