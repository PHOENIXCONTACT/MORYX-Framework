using System;

namespace Marvin.Runtime.Modules
{
    internal class StartingState : ServerModuleStateBase
    {
        public StartingState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Starting)
        {
        }

        public override void OnEnter()
        {
            try
            {
                Context.Start();
                NextState(StateRunning);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                NextState(StateRunningFailure);
            }
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // We are already starting
        }

        public override void Stop()
        {
            // Nothing to do here
        }
    }
}
