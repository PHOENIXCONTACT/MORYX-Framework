using System;

namespace Marvin.Runtime.Modules
{
    internal class StoppingState : ServerModuleStateBase
    {
        public StoppingState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Stopping)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            try
            {
                Context.Stop();
                NextState(StateStopped);
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
            // Not possible here
        }

        public override void Stop()
        {
            // We are already stopping
        }

        public override void ErrorOccured()
        {
            NextState(StateFailure);
        }
    }
}