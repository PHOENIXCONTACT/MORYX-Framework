using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Configuration for the Proxy.
    /// </summary>
    [DataContract]
    public class ProxyConfig : ConfigBase, IProxyConfig
    {
        /// <summary>
        /// Config entry to enable / disable the proxy.
        /// </summary>
        [DataMember]
        public bool EnableProxy { get; set; }

        /// <summary>
        /// Config entry to enable / disable the use of the default web proxy.
        /// </summary>
        [DataMember]
        public bool UseDefaultWebProxy { get; set; }

        /// <summary>
        /// Config entry for the proxy adress.
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// config entry for the proxy port.
        /// </summary>
        [DataMember]
        public int Port { get; set; }
    }
}