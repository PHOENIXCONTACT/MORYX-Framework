using System;

namespace Marvin.Runtime.Modules
{
    internal class StartingState : ServerModuleStateBase
    {
        public StartingState(IStateBasedTransitions context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Starting)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            try
            {
                Context.Start();
                NextState(StateRunning);
            }
            catch (Exception ex)
            {
                Context.ReportFailure(Context, ex);
            }
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Stop()
        {
            // Nothing to do here
        }

        public override void ErrorOccured()
        {
            NextState(StateFailure);
        }
    }
}
