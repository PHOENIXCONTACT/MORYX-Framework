namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Public API of the WCF client factory configuration.
    /// </summary>
    public class WcfClientFactoryConfig : IWcfClientFactoryConfig
    {
        /// 
        public string Host { get; set; }

        /// 
        public int Port { get; set; }

        /// 
        public string ClientId { get; set; }
    }
}