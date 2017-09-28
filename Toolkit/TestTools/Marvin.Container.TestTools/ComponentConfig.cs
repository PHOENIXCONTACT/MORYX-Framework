using Marvin.Modules.ModulePlugins;

namespace Marvin.Container.TestTools
{
    public class ComponentConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public string PluginName { get; set; }
    }
}