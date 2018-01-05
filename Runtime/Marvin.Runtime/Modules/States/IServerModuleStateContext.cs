using Marvin.Modules;
using Marvin.StateMachines;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Interface for server module base to access state based transitions
    /// </summary>
    internal interface IServerModuleStateContext : IStateContext, IModuleErrorReporting
    {
        /// <summary>
        /// Initialize the module
        /// </summary>
        void Initialize();

        /// <summary>
        /// Start the module
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the module
        /// </summary>
        void Stop();

        /// <summary>
        /// Adds a notification to the module
        /// </summary>>
        void LogNotification(object sender, IModuleNotification notification);

        /// <summary>
        /// Called if the health state is invalid for the current state
        /// </summary>
        void InvalidHealthState(ServerModuleState state);
    }
}
