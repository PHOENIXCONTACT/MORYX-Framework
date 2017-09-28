using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.TestModule
{
    [Plugin(LifeCycle.Singleton, typeof(IAnotherPlugin))]
    public class AnotherPlugin : IAnotherPlugin
    {
        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(AnotherPluginConfig config)
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

    [ExpectedConfig(typeof(AnotherPluginConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(IAnotherPlugin))]
    public class AnotherPlugin2 : IAnotherPlugin
    {
        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(AnotherPluginConfig config)
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

    [Plugin(LifeCycle.Singleton, typeof(IAnotherSubPlugin))]
    public class AnotherSubPlugin : IAnotherSubPlugin
    {
        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(AnotherSubConfig config)
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

    [ExpectedConfig(typeof(AnotherSubConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(IAnotherSubPlugin))]
    public class AnotherSubPlugin2 : IAnotherSubPlugin
    {
        /// <summary>
        /// Initialize this component with its config
        /// </summary>
        /// <param name="config">Config of this module plugin</param>
        public void Initialize(AnotherSubConfig config)
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