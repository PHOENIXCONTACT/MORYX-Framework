using System;

namespace Marvin.Runtime.Modules
{
    internal class RunningStoppingState : StoppingStateBase
    {
        public RunningStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override void OnStopping()
        {
            Context.Stop();
            Context.Destruct();
        }

        public override void ErrorOccured()
        {
            NextState(StateRunningFailure);
        }
    }

    internal class ReadyStoppingState : StoppingStateBase
    {
        public ReadyStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override void OnStopping()
        {
            Context.Destruct();
        }

        public override void ErrorOccured()
        {
            NextState(StateInitializedFailure);
        }
    }

    internal abstract class StoppingStateBase : ServerModuleStateBase
    {
        protected StoppingStateBase(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Stopping)
        {
        }

        public override void OnEnter()
        {
            try
            {
                OnStopping();
                NextState(StateStopped);
            }
            catch (Exception ex)
            {
                Context.ReportFailure(ex);
            }
        }

        protected abstract void OnStopping();

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Not possible here
        }

        public override void Stop()
        {
            // We are already stopping
        }

        public abstract override void ErrorOccured();
    }
}