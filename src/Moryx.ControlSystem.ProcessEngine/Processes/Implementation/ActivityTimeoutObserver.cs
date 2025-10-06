// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.Notifications;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener))]
    internal sealed class ActivityTimeoutObserver : IActivityPoolListener, INotificationSender, IDisposable
    {
        private readonly IDictionary<ActivityData, int> _runningTimers = new ConcurrentDictionary<ActivityData, int>();

        /// <inheritdoc />
        public int StartOrder => 120;

        string INotificationSender.Identifier => nameof(ActivityTimeoutObserver);

        #region Dependencies

        public IActivityDataPool ActivityPool { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public INotificationAdapter NotificationAdapter { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            ActivityPool.ActivityChanged += OnActivityChanged;
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (_runningTimers)
                foreach (var runningTimer in _runningTimers.Values)
                    ParallelOperations.StopExecution(runningTimer);

            _runningTimers.Clear();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ActivityPool.ActivityChanged -= OnActivityChanged;
        }

        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            var timeoutParameters = args.ActivityData.Activity.Parameters as IActivityTimeoutParameters;
            if (timeoutParameters == null || timeoutParameters.Timeout <= 0)
                return;

            switch (args.Trigger)
            {
                case ActivityState.Running:
                    ActivityRunning(args.ActivityData, timeoutParameters);
                    break;
                case ActivityState.ResultReceived:
                    ActivityResultReceived(args.ActivityData);
                    break;
            }
        }

        private void ActivityRunning(ActivityData activityData, IActivityTimeoutParameters timeoutParameters)
        {
            var timerId = ParallelOperations.ScheduleExecution(OnActivityTimeout, activityData,
                timeoutParameters.Timeout * 1000, -1);

            _runningTimers[activityData] = timerId;
        }

        private void OnActivityTimeout(ActivityData activityData)
        {
            _runningTimers.Remove(activityData);

            var activityName = activityData.Activity.Name;

            var timeoutParameters = (IActivityTimeoutParameters)activityData.Activity.Parameters;
            var notification = new Notification(Strings.ActivityTimeoutObserver_ActivityTimeout_Title,
                string.Format(Strings.ActivityTimeoutObserver_ActivityTimeout_Message, activityName,
                    timeoutParameters.Timeout), ModuleConfig.ActivityTimeoutSeverity, true);

            NotificationAdapter.Publish(this, notification, activityData);
        }

        void INotificationSender.Acknowledge(Notification notification, object tag)
        {
            NotificationAdapter.Acknowledge(this, notification);
        }

        private void ActivityResultReceived(ActivityData activityData)
        {
            NotificationAdapter.AcknowledgeAll(this, activityData);

            if (!_runningTimers.ContainsKey(activityData))
                return;

            ParallelOperations.StopExecution(_runningTimers[activityData]);
            _runningTimers.Remove(activityData);
        }
    }
}
