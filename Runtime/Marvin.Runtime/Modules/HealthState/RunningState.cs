using System;

namespace Marvin.Runtime.Modules
{
    internal class RunningState : HealthState
    {
        /// <summary>
        /// The most liked state. It represents that the module is running and working correct without errors.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController.</param>
        public RunningState(IStateBasedTransitions target, HealthStateController context) 
            : base(target, context, ServerModuleState.Running)
        {
        }

        /// <summary>
        /// Change state dependend on critical error. If critical, then failure state otherwhise waring state.
        /// </summary>
        /// <param name="criticalError">flag for the critical error. </param>
        public override void ErrorOccured(bool criticalError)
        {
            var newState = criticalError ? new FailureState(Target, Context) : (HealthState)new WarningState(Target, Context);
            Context.UpdateState(newState);
        }

        /// <summary>
        /// Try to stop a module. Change to stopped state on success.
        /// </summary>
        public override void Stop()
        {
            try
            {
                Context.Current = ServerModuleState.Stopping;
                Target.Stop();
                Context.UpdateState(new StoppedState(Target, Context));
            }
            catch (Exception ex)
            {
                Context.ReportFailure(this, ex);
            }
        }
    }
}
