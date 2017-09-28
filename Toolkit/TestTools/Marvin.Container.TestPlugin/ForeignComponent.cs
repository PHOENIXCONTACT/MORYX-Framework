using Marvin.Container.TestTools;

namespace Marvin.Container.TestPlugin
{
    [Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
    public class ForeignComponent : IConfiguredComponent
    {
        public const string PluginName = "ForeignComponent";

        public string GetName()
        {
            return PluginName;
        }

        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(ComponentConfig config)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
        }
    }
}