using System;
using System.Threading;
using Marvin.Modules.Server;
using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.Base.HealthState
{
    internal class RetryState : HealthState
    {
        private int _retryCount;

        private readonly MissingDependencyException _firstException;
        private readonly Timer _retryTimer;

        /// <summary>
        /// Try to start a module again. Will do this MissingDependencyException.Retries times 
        /// and wait MissingDependencyException.AwaitTimeMs between tries.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController.</param>
        /// <param name="ex">The MissingDependencyException which occured before switched to this state.</param>
        public RetryState(IStateBasedTransitions target, HealthStateController context, MissingDependencyException ex)
            : base(target, context, ServerModuleState.Starting)
        {
            _firstException = ex;
            CreateNotification(ex);

            _retryTimer = new Timer(RetryStart, null, ex.AwaitTimeMs, -1);
        }

        private void RetryStart(object state)
        {
            Exception finalEx = null;
            try
            {
                // Call start again to see if it goes through now
                Target.Start();
                
                // If we made it here the Start was successful
                Context.UpdateState(new RunningState(Target, Context));
            }
            catch (MissingDependencyException missEx)
            {
                _retryCount++;
                if (_retryCount <= _firstException.Retries)
                    _retryTimer.Change(missEx.AwaitTimeMs, -1);
                else
                    finalEx = missEx;
            }
            catch (Exception ex)
            {
                finalEx = ex;
            }

            if(finalEx == null)
                return;

            _retryTimer.Dispose();
            Context.ReportFailure(this, finalEx);
        }

        /// <summary>
        /// Change state to failure when a critical error occured.
        /// </summary>
        /// <param name="criticalError">Flag for setting critical error or not.</param>
        public override void ErrorOccured(bool criticalError)
        {
            if (criticalError)
                Context.UpdateState(new FailureState(Target, Context));
        }

        /// <summary>
        /// Try to stop the module and switch to stopped state on success. 
        /// </summary>
        public override void Stop()
        {
            try
            {
                _retryTimer.Dispose();
                Context.Current = ServerModuleState.Stopping;
                Target.Stop();
                Context.UpdateState(new StoppedState(Target, Context));
            }
            catch (Exception ex)
            {
                Context.ReportFailure(this, ex);
            }
        }

        private void CreateNotification(Exception missEx)
        {
            Context.LogNotification(Context, new WarningNotification(notification => Context.Notifications.Remove(notification), 
                                       missEx, "An external dependency of the module is missing."));
        }
    }
}
