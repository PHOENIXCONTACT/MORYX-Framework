namespace Marvin.Runtime.Modules
{
    internal class RunningState : ServerModuleStateBase
    {
        public RunningState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap, ServerModuleState.Running)
        {
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Already started
        }

        public override void Stop()
        {
            NextState(StateStopping);
        }

        public override void ErrorOccured()
        {
            NextState(StateFailure);
        }

        public override void ValidateHealthState()
        {
            // Health state should be okay!
        }
    }
}