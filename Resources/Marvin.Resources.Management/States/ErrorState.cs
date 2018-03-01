namespace Marvin.Resources.Management
{
    /// <summary>
    /// State of a <see cref="ResourceWrapper"/> when an error occurred while acting on wrapped resource
    /// </summary>
    internal class ErrorState : ResourceStateBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ErrorState(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            // Do nothing
        }

        /// <inheritdoc />
        public override void Start()
        {
            // Do nothing
        }
    }
}
