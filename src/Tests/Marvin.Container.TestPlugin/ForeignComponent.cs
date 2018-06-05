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

        /// <inheritdoc />
        public void Initialize(ComponentConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }
    }
}