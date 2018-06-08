namespace Marvin.Resources.Management
{
    /// <summary>
    /// State of the <see cref="ResourceWrapper"/> when the wrappd resource was initialized
    /// </summary>
    internal class InitializedState : ResourceStateBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public InitializedState(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <inheritdoc />
        public override void Start()
        {
            Context.HandleStart();
            NextState(StateStarted);
        }
    }
}
