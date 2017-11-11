using Marvin.Configuration;
using Marvin.Modules;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Basic configuration for WCF service plugins
    /// </summary>
    public interface IWcfServiceConfig : IUpdatableConfig, IPluginConfig
    {
        /// <summary>
        /// The WCF host configuration
        /// </summary>
        HostConfig ConnectorHost { get; }
    }
}