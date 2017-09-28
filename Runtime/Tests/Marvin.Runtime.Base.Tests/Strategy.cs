using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.Base.Tests
{
    public interface IStrategy
    {
    }

    public class StrategyConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public string PluginName { get; set; }
    }
}