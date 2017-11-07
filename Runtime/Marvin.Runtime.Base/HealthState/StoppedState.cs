using System;

namespace Marvin.Runtime.Base.HealthState
{
    internal class StoppedState : HealthState
    {
        public StoppedState(IStateBasedTransitions target, HealthStateController context) 
            : base(target, context, ServerModuleState.Stopped)
        {
        }

        /// <summary>
        /// Handle state switch when an error occured.
        /// Stopped modules will be allways handles as critical error since they should not execute code.
        /// </summary>
        /// <param name="criticalError">flag which set if it is a critical error or can be handled as warning.</param>
        public override void ErrorOccured(bool criticalError)
        {
            // Stopped modules should not execute code so this is allways a critical error
            HealthState newState;
            if (Context.Current == ServerModuleState.Ready)
                newState = new FailureState(Target, Context);
            else
                newState = criticalError ? new FailureState(Target, Context) : (HealthState)new WarningState(Target, Context);

            Context.UpdateState(newState);
        }

        /// <summary>
        /// Try to initialize the target. 
        /// </summary>
        public override void Initialize()
        {
            try
            {
                Context.Current = ServerModuleState.Initializing;
                Target.Initialize();
                Context.UpdateState(new ReadyState(Target, Context));
            }
            catch (Exception ex)
            {
                Context.ReportFailure(this, ex);
            }
        }
    }
}
