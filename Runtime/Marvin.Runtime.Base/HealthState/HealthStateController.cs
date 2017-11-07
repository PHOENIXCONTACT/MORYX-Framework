using System;
using System.Threading;
using Marvin.Logging;
using Marvin.Modules;

namespace Marvin.Runtime.Base.HealthState
{
    /// <summary>
    /// State controller managing the entire modules life cycle
    /// </summary>
    internal class HealthStateController : ILoggingComponent, IModuleErrorReporting, IServerModuleState
    {
        private readonly IStateBasedTransitions _target;
        private HealthState _state;

        /// <summary>
        /// Create a new state controller passing in a component with Initialize, Start and Stop method
        /// </summary>
        public HealthStateController(IStateBasedTransitions target)
        {
            _target = target;
            UpdateState(new StoppedState(target, this));

            Notifications = new ServerNotificationCollection();
        }

        /// <summary>
        /// Logger of this component
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #region State change

        /// <summary>
        /// Updates the state to the new instance.
        /// </summary>
        /// <param name="newState">The new state which should be updated to.</param>
        public void UpdateState(HealthState newState)
        {
            _state = newState;
        }

        /// <summary>
        /// Event which will inform when a state change has occured.
        /// </summary>
        public event EventHandler<ModuleStateChangedEventArgs> Changed;
        private void StateChange(ServerModuleState oldState, ServerModuleState newState)
        {
            if (Changed == null || oldState == newState)
                return;

            // Since event handling may take a while make sure we don't stop module execution
            foreach (var caller in Changed.GetInvocationList())
            {
                ThreadPool.QueueUserWorkItem(delegate(object callObj)
                {
                    try
                    {
                        var callDelegate = (Delegate)callObj;
                        callDelegate.DynamicInvoke(_target, new ModuleStateChangedEventArgs {OldState = oldState, NewState = newState});
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogException(LogLevel.Warning, ex, "Failed to notify listener of state change");
                    }
                }, caller);
            }
        }

        private ServerModuleState _current;

        /// <summary>
        /// The current state.
        /// </summary>
        public ServerModuleState Current
        {
            get { return _current; }
            set
            {
                var oldState = _current;
                _current = value;
                StateChange(oldState, value);
            }
        }

        #endregion

        #region Transitions

        /// <summary>
        /// Initialize called over module API forwarded to current state
        /// </summary>
        public void Initialize()
        {
            lock (this)
                _state.Initialize();
        }

        /// <summary>
        /// Start called over module API forwarded to current state
        /// </summary>
        public void Start()
        {
            lock (this)
                _state.Start();
        }

        /// <summary>
        /// Stop called over module API forwarded to current state
        /// </summary>
        public void Stop()
        {
            lock (this)
                _state.Stop();
        }

        #endregion

        #region ErrorReporting
        /// <summary>
        /// Notifications raised within module and during state changes
        /// </summary>
        public ServerNotificationCollection Notifications { get; private set; }

        /// <summary>
        /// Report internal failure to parent module
        /// </summary>
        /// <param name="sender">The sender of the failure report.</param>
        /// <param name="exception">The exception which should be reported.</param>
        public void ReportFailure(object sender, Exception exception)
        {
            var notification = new FailureNotification(exception, string.Format("Component {0} reported an exception", sender.GetType().Name));
            LogNotification(sender, notification);
            _state.ErrorOccured(true);
        }

        /// <summary>
        /// Report an error to be treated as a warning
        /// </summary>
        /// <param name="sender">The sender of the warning report.</param>
        /// <param name="exception">The exception which should be reported as warning.</param>
        public void ReportWarning(object sender, Exception exception)
        {
            var notification = new WarningNotification(n => Notifications.Remove(n), exception, string.Format("Component {0} reported an exception", sender.GetType().Name));
            LogNotification(sender, notification);
            _state.ErrorOccured(false);
        }

        internal void LogNotification(object sender, IModuleNotification notification)
        {
            Notifications.Add(notification);

            var logger = sender is ILoggingComponent ? ((ILoggingComponent)sender).Logger : Logger;
            logger.LogException(notification.Type == NotificationType.Warning ? LogLevel.Warning : LogLevel.Error,
                                notification.Exception, notification.Message);
        }

        internal void ClearNotifications()
        {
            Notifications.Clear();
        }
        #endregion
    }
}
