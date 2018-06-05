using System;

namespace Marvin.Runtime.Modules
{
    internal class InitializedFailureState : FailureStateBase
    {
        public InitializedFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override void OnFailure()
        {
            Context.Destruct();
        }
    }

    internal class RunningFailureState : FailureStateBase
    {
        public RunningFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override void OnFailure()
        {
            Context.Stop();
            Context.Destruct();
        }
    }

    internal abstract class FailureStateBase : ServerModuleStateBase
    {
        protected FailureStateBase(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Failure)
        {
        }

        public override void OnEnter()
        {
            try
            {
                OnFailure();
            }
            catch (Exception ex)
            {
                Context.LogNotification(Context, new FailureNotification(ex, "Failed to stop faulty module!"));
            }
        }

        protected abstract void OnFailure();

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