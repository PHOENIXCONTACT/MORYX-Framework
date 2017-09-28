using Marvin.Modules.ModulePlugins;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// The public API of the WCF connector plugin.
    /// </summary>
    public interface IWcfConnector<TConfig> : IConfiguredModulePlugin<TConfig>
        where TConfig : IWcfServiceConfig
    {
         
    }
}