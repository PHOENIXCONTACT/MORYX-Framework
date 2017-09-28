using System;
using System.Collections.Specialized;
using Marvin.Modules.Server;

namespace Marvin.Runtime.Base.HealthState
{
    internal class WarningState : HealthState
    {
        /// <summary>
        /// State to represents a warning state. The module had an error but it will not stop it from working.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController.</param>
        public WarningState(IStateBasedTransitions target, HealthStateController context)
            : base(target, context, ServerModuleState.Warning)
        {
            // Register listener to make detect notification confirmation
            Context.Notifications.CollectionChanged += NotifcationsChanged;
        }

        private void NotifcationsChanged(object sender, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
        {
            if (Context.Notifications.Count == 0)
                SwitchState(new RunningState(Target, Context));
        }

        /// <summary>
        /// Switch to another state but first unregister from event
        /// </summary>
        private void SwitchState(HealthState newState)
        {
            Context.Notifications.CollectionChanged -= NotifcationsChanged;
            Context.UpdateState(newState);
        }

        /// <summary>
        /// Change state to failure when a critical error occured. A non critical error will not change the state.
        /// </summary>
        /// <param name="criticalError">Flag to determine the critical error.</param>
        public override void ErrorOccured(bool criticalError)
        {
            if (criticalError)
                Context.UpdateState(new FailureState(Target, Context));
        }

        /// <summary>
        /// Try to stop the modle. Change state to stopped on success.
        /// </summary>
        public override void Stop()
        {
            try
            {
                Context.Current = ServerModuleState.Stopping;
                Target.Stop();
                SwitchState(new StoppedState(Target, Context));
            }
            catch (Exception ex)
            {
                Context.ReportFailure(this, ex);
            }
        }
    }
}
