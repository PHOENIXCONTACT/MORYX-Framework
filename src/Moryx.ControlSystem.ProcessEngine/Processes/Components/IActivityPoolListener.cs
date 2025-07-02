using Moryx.Modules;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Shared interface for all components that listen to the activity pool
    /// </summary>
    internal interface IActivityPoolListener : IInitializablePlugin
    {
        /// <summary>
        /// Determines the boot and shutdown order
        /// </summary>
        int StartOrder { get; }
    }
}