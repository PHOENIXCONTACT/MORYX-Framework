using Marvin.StateMachines;

namespace Marvin.Runtime.Modules
{
    internal abstract class ServerModuleStateBase : StateBase<IServerModuleStateContext>
    {
        public ServerModuleState Classification { get; }

        protected ServerModuleStateBase(IServerModuleStateContext context, StateMap stateMap, ServerModuleState classification) : base(context, stateMap)
        {
            Classification = classification;
        }

        public virtual void Initialize()
        {
            InvalidState();
        }

        public virtual void Start()
        {
            InvalidState();
        }

        public virtual void Stop()
        {
            InvalidState();
        }

        public virtual void ErrorOccured()
        {
            InvalidState();
        }

        public virtual void ValidateHealthState()
        {
            Context.InvalidHealthState(Classification);
        }

        [StateDefinition(typeof(StoppedState), IsInitial = true)]
        protected const int StateStopped = 0;

        [StateDefinition(typeof(InitializingState))]
        protected const int StateInitializing = 10;

        [StateDefinition(typeof(ReadyState))]
        protected const int StateReady = 20;

        [StateDefinition(typeof(StartingState))]
        protected const int StateStarting = 30;

        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 40;

        [StateDefinition(typeof(StoppingState))]
        protected const int StateStopping = 50;

        [StateDefinition(typeof(FailureState))]
        protected const int StateFailure = 60;
    }
}