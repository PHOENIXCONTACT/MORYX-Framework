using System;
using Marvin.Modules.Server;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.Base.HealthState
{
    internal class ReadyState : HealthState
    {
        /// <summary>
        /// Represents that the target is ready to work but is not running until now.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController</param>
        public ReadyState(IStateBasedTransitions target, HealthStateController context)
            : base(target, context, ServerModuleState.Ready)
        {
            Context.ClearNotifications();
        }

        /// <summary>
        /// Change to failure state because ready modules should not throw errors. 
        /// </summary>
        /// <param name="criticalError">Flag for critical or not critical error.</param>
        public override void ErrorOccured(bool criticalError)
        {
            // Ready modules should not be throwing errors
            HealthState newState;
            if (Context.Current == ServerModuleState.Ready)
                newState = new FailureState(Target, Context);
            else
                newState = criticalError ? new FailureState(Target, Context) : (HealthState)new WarningState(Target, Context);
            
            Context.UpdateState(newState);
        }

        /// <summary>
        /// Try to start the module and switch to running state on success. Switch to retry state when a MissingDependencyException occured.
        /// </summary>
        public override void Start()
        {
            try
            {
                Context.Current = ServerModuleState.Starting;
                Target.Start();
                Context.UpdateState(new RunningState(Target, Context));
            }
            catch (MissingDependencyException missEx)
            {
                Context.UpdateState(new RetryState(Target, Context, missEx));
            }
            catch (Exception ex)
            {
                Context.ReportFailure(this, ex);
            }
        }

        /// <summary>
        /// Try to stop the module and switch to stopped state on success.
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
