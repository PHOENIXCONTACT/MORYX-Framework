using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Runtime
{
    /// <summary>
    /// Configuration of product executed in HeartOfGold
    /// </summary>
    [DataContract]
    public class ProductConfig : IConfig
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ProductConfig"/>
        /// </summary>
        public ProductConfig()
        {
            Version = RuntimePlatform.RuntimeVersion;
        }

        /// <summary>
        /// <see cref="ConfigState"/>
        /// </summary>
        public ConfigState ConfigState { get; set; }

        /// <summary>
        /// Exception message if load failed
        /// </summary>
        [ReadOnly(true)]
        public string LoadError { get; set; }

        /// <summary>
        /// Name of this product
        /// </summary>
        [DataMember]
        [DefaultValue("MarvinRuntime")]
        public string ProductName { get; set; }

        /// <summary>
        /// Version of this product
        /// </summary>
        [DataMember]
        public string Version { get; set; }
    }
}