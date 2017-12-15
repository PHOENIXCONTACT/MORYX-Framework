using System;

namespace Marvin.Runtime.Modules
{
    internal class InitializingState : ServerModuleStateBase
    {
        public InitializingState(IStateBasedTransitions context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Initializing)
        {
        }

        public override void OnEnter()
        {
            try
            {
                Context.Initialize();
                NextState(StateReady);
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

        public override void ErrorOccured()
        {
            NextState(StateFailure);
        }
    }
}
