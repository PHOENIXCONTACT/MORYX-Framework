namespace Marvin.Runtime.Modules
{
    internal class StoppedState : ServerModuleStateBase
    {
        public StoppedState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Stopped)
        {
        }

        public override void Initialize()
        {
            NextState(StateInitializing);
        }

        public override void Stop()
        {
            // Stop again doent matter
        }
    }
}
