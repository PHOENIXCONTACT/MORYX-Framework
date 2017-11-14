using System;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Interface to access the modules state, state history and register to change events
    /// </summary>
    public interface IServerModuleState
    {
        /// <summary>
        /// Current state of the server module
        /// </summary>
        ServerModuleState Current { get; }

        /// <summary>
        /// Event raised when the current state changes
        /// </summary>
        event EventHandler<ModuleStateChangedEventArgs> Changed;
    }

    /// <summary>
    /// Event args for the StateChanged event
    /// </summary>
    public class ModuleStateChangedEventArgs
    {
        /// <summary>
        /// Old state of the module
        /// </summary>
        public ServerModuleState OldState { get; set; }

        /// <summary>
        /// New state of the module
        /// </summary>
        public ServerModuleState NewState { get; set; }
    }
}
