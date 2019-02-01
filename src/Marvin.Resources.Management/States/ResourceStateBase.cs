using Marvin.StateMachines;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Base class for states of the <see cref="ResourceWrapper"/>
    /// </summary>
    internal abstract class ResourceStateBase : StateBase<ResourceWrapper>
    {
        /// <summary>
        /// constructor
        /// </summary>
        protected ResourceStateBase(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Flag if the resource is avaiable for external interaction
        /// </summary>
        public virtual bool IsAvailable => false;

        /// <summary>
        /// Initialize the wrapped resource
        /// </summary>
        public virtual void Initialize()
        {
            InvalidState();
        }

        /// <summary>
        /// Start the wrapped resource
        /// </summary>
        public virtual void Start()
        {
            InvalidState();
        }
        /// <summary>
        /// Handle an error and set the wrapped resource to the error state
        /// </summary>
        public virtual void ErrorOccured()
        {
            NextState(StateError);
        }

        /// <summary>
        /// Stop the wrapped resource
        /// </summary>
        public virtual void Stop()
        {
            Context.HandleStop();
            NextState(StateStopped);
        }

        /// <summary>
        /// The wrapped resource was created
        /// </summary>
        [StateDefinition(typeof(CreatedState), IsInitial = true)]
        protected const int StateCreated = 0;

        /// <summary>
        /// The wrapped resource was initialized
        /// </summary>
        [StateDefinition(typeof(InitializedState))]
        protected const int StateInitialized = 10;

        /// <summary>
        /// The wrapped resource was started
        /// </summary>
        [StateDefinition(typeof(StartedState))]
        protected const int StateStarted = 20;

        /// <summary>
        /// The wrapped resource was stopped
        /// </summary>
        [StateDefinition(typeof(StoppedState))]
        protected const int StateStopped = 30;

        /// <summary>
        /// An error occurred while performing an action on the wrapped resource.
        /// </summary>
        [StateDefinition(typeof(ErrorState))]
        protected const int StateError = 40;
    }
}
