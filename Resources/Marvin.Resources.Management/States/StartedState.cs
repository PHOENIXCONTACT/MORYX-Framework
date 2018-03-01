namespace Marvin.Resources.Management
{
    /// <summary>
    /// State of the <see cref="ResourceWrapper"/> when the wrappd resource was started
    /// </summary>
    internal class StartedState : ResourceStateBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public StartedState(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }
    }
}
