using System;

namespace Marvin.Runtime.Modules
{
    internal class FailureState : ServerModuleStateBase
    {
        public FailureState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Failure)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            try
            {
                Context.Stop();
            }
            catch (Exception ex)
            {
                Context.LogNotification(Context, new FailureNotification(ex, "Failed to stop faulty module!"));
            }
        }

        public override void Initialize()
        {
            NextState(StateInitializing);
        }

        public override void Stop()
        {
            NextState(StateStopped);
        }

        public override void ErrorOccured()
        {
            // Nothing to do here. We are already in Failure
        }
    }
}