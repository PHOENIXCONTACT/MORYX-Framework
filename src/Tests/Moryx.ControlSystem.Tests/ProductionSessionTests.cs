// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using NUnit.Framework;

namespace Moryx.ControlSystem.Tests
{

    [TestFixture]
    public class ProductionSessionTests
    {
        [Test]
        public void TestProcessPresent1()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 1);

            Assert.That(readyToWork.Reference.HasReference, Is.True);
            Assert.That(readyToWork.Reference.IsEmpty, Is.False);
        }

        [Test]
        public void TestArticlePresent2()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull);

            Assert.That(readyToWork.Reference.HasReference, Is.False);
            Assert.That(readyToWork.Reference.IsEmpty, Is.True);
        }

        [TestCase(ReadyToWorkType.Pull)]
        [TestCase(ReadyToWorkType.Push)]
        public void TestStartSession(ReadyToWorkType readyToWorkType)
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, readyToWorkType, 4242);

            Assert.That(readyToWork.Reference, Is.EqualTo(ProcessReference.ProcessId(4242)));
            Assert.That(readyToWork.ReadyToWorkType, Is.EqualTo(readyToWorkType));
        }

        [Test]
        public void TestCreateActivityStart()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });

            Assert.That(activityStart.Reference, Is.EqualTo(readyToWork.Reference));
            Assert.That(activityStart.Id, Is.EqualTo(readyToWork.Id));
            Assert.That(readyToWork.Process, Is.Not.EqualTo(activityStart.Process)); // Make sure process is not populated back to RTW
        }

        [Test]
        public void TestPauseSession()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var notReadyToWork = readyToWork.PauseSession();

            Assert.That(notReadyToWork.Reference, Is.EqualTo(readyToWork.Reference));
            Assert.That(notReadyToWork.Id, Is.EqualTo(readyToWork.Id));
        }

        [TestCase(ReadyToWorkType.Pull)]
        [TestCase(ReadyToWorkType.Push)]
        public void TestResumeSession(ReadyToWorkType rtwType)
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var notReadyToWork = readyToWork.PauseSession();

            var readyToWork2 = notReadyToWork.ResumeSession();

            Assert.That(notReadyToWork.Reference, Is.EqualTo(readyToWork2.Reference));
            Assert.That(notReadyToWork.Id, Is.EqualTo(readyToWork2.Id));
        }

        [Test]
        public void TestCreateActivityResult()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });
            activityStart.Activity.Complete(1);

            var activityCompleted = activityStart.CreateResult();

            Assert.That(activityCompleted.Reference, Is.EqualTo(readyToWork.Reference));
            Assert.That(activityCompleted.Id, Is.EqualTo(readyToWork.Id));
        }

        [Test]
        public void TestCompleteSequence()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });
            activityStart.Activity.Complete(1);

            var activityCompleted = activityStart.CreateResult();

            var sequenceCompleted = activityCompleted.CompleteSequence(activityStart.Process, true);

            Assert.That(sequenceCompleted.Reference, Is.EqualTo(readyToWork.Reference));
            Assert.That(sequenceCompleted.Id, Is.EqualTo(readyToWork.Id));
        }

        [Test]
        public void TestThrowArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(delegate
            {
                Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242, null);
            });
        }

        [TestCase(ReadyToWorkType.Pull)]
        [TestCase(ReadyToWorkType.Push)]
        public void TestContinueSession(ReadyToWorkType readyToWorkType)
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, readyToWorkType, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });
            activityStart.Activity.Complete(1);

            var activityCompleted = activityStart.CreateResult();

            var sequenceCompleted = activityCompleted.CompleteSequence(activityStart.Process, true);

            var readyToWork2 = sequenceCompleted.ContinueSession();

            Assert.That(readyToWork2.Reference, Is.EqualTo(readyToWork.Reference));
            Assert.That(readyToWork2.Id, Is.EqualTo(readyToWork.Id));
            Assert.That(readyToWork.ReadyToWorkType, Is.EqualTo(readyToWorkType));
            Assert.That(readyToWork2.ReadyToWorkType, Is.EqualTo(ReadyToWorkType.Pull));
        }

        [Test]
        public void TestUnknownActivityAborted()
        {
            // Arrange
            var process = new Process { Id = 4242 };
            var activity = new DummyActivity { Process = process };

            // Act
            var session = Session.WrapUnknownActivity(activity);

            // Assert
            Assert.That(session.AcceptedClassification, Is.EqualTo(ActivityClassification.Unknown));
            Assert.That(session.AbortedActivity, Is.EqualTo(activity));
            Assert.That(session.Reference.Matches(process));
        }
    }
}
