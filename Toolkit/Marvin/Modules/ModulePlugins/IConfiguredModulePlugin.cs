namespace Marvin.Modules.ModulePlugins
{
    /// <summary>
    /// This generic interface is intended for all plugins that require a configuration for their initialization to work properly. 
    /// This configuration is passed to the plugin via the Initialize(TConf config) method. 
    /// </summary>
    public interface IConfiguredModulePlugin<in T> : IModulePlugin
        where T : IPluginConfig
    {
        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        void Initialize(T config);
    }
}
