using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Communication
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
        /// Config entry for the proxy address.
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