namespace Marvin.Runtime.Modules
{
    internal class ReadyState : ServerModuleStateBase
    {
        public ReadyState(IStateBasedTransitions context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Ready)
        {
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            NextState(StateStarting);
        }

        public override void Stop()
        {
            NextState(StateStopping);
        }

        public override void ErrorOccured()
        {

        }
    }
}