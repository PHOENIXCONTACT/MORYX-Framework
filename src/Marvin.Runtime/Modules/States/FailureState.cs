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
        }
    }

    internal class RunningFailureState : FailureStateBase
    {
        public RunningFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override void OnFailure()
        {
            try
            {
                Context.Stop();
            }
            catch (Exception ex)
            {
                Context.LogNotification(new FailureNotification(ex, "Failed to stop faulty module!"));
            }
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
            OnFailure();

            try
            {
                // Regardless of the previous step we need to try destroying the container
                // This can still cause Exceptions in Dispose
                Context.Destruct();
            }
            catch (Exception ex)
            {
                Context.LogNotification(new FailureNotification(ex, "Failed to destroy faulty container!"));
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
    }
}