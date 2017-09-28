using Marvin.Container.TestTools;

namespace Marvin.Container.Tests
{
    [Plugin(LifeCycle.Singleton, typeof(IRootClass), Name = PluginName)]
    internal class RootClass : IRootClass
    {
        internal const string PluginName = "RootClass";

        // Injected
        public IConfiguredComponent ConfiguredComponent { get; set; }

        public string GetName()
        {
            return PluginName;
        }

        public void Initialize(RootClassFactoryConfig config)
        {
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }

    [Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
    internal class ConfiguredComponentA : IConfiguredComponent
    {
        internal const string PluginName = "ConfiguredA";

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

    [Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
    internal class ConfiguredComponentB : IConfiguredComponent
    {
        internal const string PluginName = "ConfiguredB";

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