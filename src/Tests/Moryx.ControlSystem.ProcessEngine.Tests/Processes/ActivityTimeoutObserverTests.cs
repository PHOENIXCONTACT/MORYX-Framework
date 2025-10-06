// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moq;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.Notifications;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ActivityTimeoutObserverTests : ProcessTestsBase
    {
        private const int TimerId = 1;
        private const int Timeout = 5;

        private ActivityTimeoutObserver _activityTimeoutObserver;
        private Mock<IParallelOperations> _parallelOpsMock;
        private Mock<IActivityDataPool> _activityPoolMock;
        private Mock<INotificationAdapter> _notificationAdapterMock;

        [SetUp]
        public void Setup()
        {
            _parallelOpsMock = new Mock<IParallelOperations>();
            _parallelOpsMock.Setup(p => p.ScheduleExecution(It.IsAny<Action<ActivityData>>(), It.IsAny<ActivityData>(), Timeout * 1000, -1))
                .Returns(TimerId);

            _activityPoolMock = new Mock<IActivityDataPool>();
            _notificationAdapterMock = new Mock<INotificationAdapter>();

            _activityTimeoutObserver = new ActivityTimeoutObserver
            {
                ParallelOperations = _parallelOpsMock.Object,
                ActivityPool = _activityPoolMock.Object,
                NotificationAdapter = _notificationAdapterMock.Object,
                ModuleConfig = new ModuleConfig()
            };

            _activityTimeoutObserver.Initialize();
        }

        [TestCase(5, true, Description = "Timeout should be started if activity gets running")]
        [TestCase(0, false, Description = "Timeout should not be started if activity gets running")]
        public void TimerShouldBeStartedIfActivityGetsRunning(int timeout, bool timeoutStarted)
        {
            // Arrange
            var activityData = new ActivityData(CreateActivity(timeout));

            // Act
            RaiseActivityChanged(activityData, ActivityState.Running);

            // Assert
            _parallelOpsMock.Verify(p => p.ScheduleExecution(It.IsAny<Action<ActivityData>>(), It.IsAny<ActivityData>(),
                timeout * 1000, -1), Times.Exactly(timeoutStarted ? 1 : 0));
        }

        [Test(Description = "Running timer should be stopped of received activity result.")]
        public void RunningTimerShouldBeStoppedOnReceivedResult()
        {
            // Arrange
            var activityData = PublishRunningActivity();

            // Act
            RaiseActivityChanged(activityData, ActivityState.ResultReceived);

            // Assert
            _notificationAdapterMock.Verify(n => n.AcknowledgeAll(_activityTimeoutObserver, activityData), Times.Once);
            _parallelOpsMock.Verify(p => p.StopExecution(TimerId), Times.Once);
        }

        [Test(Description = "Notification should be published on reached timeout.")]
        public void NotificationShouldBePublishedOnReachedTimeout()
        {
            // Arrange
            Action<ActivityData> callback = null;
            _parallelOpsMock.Setup(p => p.ScheduleExecution(It.IsAny<Action<ActivityData>>(), It.IsAny<ActivityData>(), Timeout * 1000, -1))
                .Callback(delegate (Action<ActivityData> action, ActivityData param, int delay, int period) { callback = action; });

            var activityData = PublishRunningActivity();

            // Act
            callback(activityData);

            // Assert
            _notificationAdapterMock.Verify(n => n.Publish(_activityTimeoutObserver, It.IsAny<Notification>(), activityData), Times.Once);
        }

        private ActivityData PublishRunningActivity()
        {
            var activityData = new ActivityData(CreateActivity(Timeout));
            RaiseActivityChanged(activityData, ActivityState.Running);

            return activityData;
        }

        private void RaiseActivityChanged(ActivityData activityData, ActivityState activityState)
        {
            _activityPoolMock.Raise(a => a.ActivityChanged += null, _activityPoolMock.Object, new ActivityEventArgs(activityData, activityState));
        }

        private DummyActivity CreateActivity(int timeout)
        {
            var activity = new DummyActivity
            {
                Parameters = new DummyActivityParameters
                {
                    Timeout = timeout
                }
            };
            return activity;
        }
    }
}

