using Marvin.Modules;

namespace Marvin.Runtime.Tests
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