using System;

namespace Marvin.Runtime.Modules
{
    internal class InitializingState : ServerModuleStateBase
    {
        public InitializingState(IServerModuleStateContext context, StateMap stateMap) 
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

        public override void Start()
        {
            // Nothing to do here
        }

        public override void Stop()
        {
            // Nothing to do here
        }

        public override void ErrorOccured()
        {
            NextState(StateInitializedFailure);
        }
    }
}
