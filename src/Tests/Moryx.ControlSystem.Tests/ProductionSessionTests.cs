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

            Assert.IsTrue(readyToWork.Reference.HasReference);
            Assert.IsFalse(readyToWork.Reference.IsEmpty);
        }

        [Test]
        public void TestArticlePresent2()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull);

            Assert.IsFalse(readyToWork.Reference.HasReference);
            Assert.IsTrue(readyToWork.Reference.IsEmpty);
        }

        [TestCase(ReadyToWorkType.Pull)]
        [TestCase(ReadyToWorkType.Push)]
        public void TestStartSession(ReadyToWorkType readyToWorkType)
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, readyToWorkType, 4242);

            Assert.AreEqual(ProcessReference.ProcessId(4242), readyToWork.Reference);
            Assert.AreEqual(readyToWorkType, readyToWork.ReadyToWorkType);
        }

        [Test]
        public void TestCreateActivityStart()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });

            Assert.AreEqual(readyToWork.Reference, activityStart.Reference);
            Assert.AreEqual(readyToWork.Id, activityStart.Id);
            Assert.AreNotEqual(readyToWork.Process, activityStart.Process); // Make sure process is not populated back to RTW
        }

        [Test]
        public void TestPauseSession()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var notReadyToWork = readyToWork.PauseSession();

            Assert.AreEqual(readyToWork.Reference, notReadyToWork.Reference);
            Assert.AreEqual(readyToWork.Id, notReadyToWork.Id);
        }

        [TestCase(ReadyToWorkType.Pull)]
        [TestCase(ReadyToWorkType.Push)]
        public void TestResumeSession(ReadyToWorkType rtwType)
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var notReadyToWork = readyToWork.PauseSession();

            var readyToWork2 = notReadyToWork.ResumeSession();

            Assert.AreEqual(readyToWork2.Reference, notReadyToWork.Reference);
            Assert.AreEqual(readyToWork2.Id, notReadyToWork.Id);
        }

        [Test]
        public void TestCreateActivityResult()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });
            activityStart.Activity.Complete(1);

            var activityCompleted = activityStart.CreateResult();

            Assert.AreEqual(readyToWork.Reference, activityCompleted.Reference);
            Assert.AreEqual(readyToWork.Id, activityCompleted.Id);
        }

        [Test]
        public void TestCompleteSequence()
        {
            var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, 4242);

            var activityStart = readyToWork.StartActivity(new DummyActivity { Process = new Process { Id = 4242 } });
            activityStart.Activity.Complete(1);

            var activityCompleted = activityStart.CreateResult();

            var sequenceCompleted = activityCompleted.CompleteSequence(activityStart.Process, true);

            Assert.AreEqual(readyToWork.Reference, sequenceCompleted.Reference);
            Assert.AreEqual(readyToWork.Id, sequenceCompleted.Id);
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

            Assert.AreEqual(readyToWork.Reference, readyToWork2.Reference);
            Assert.AreEqual(readyToWork.Id, readyToWork2.Id);
            Assert.AreEqual(readyToWorkType, readyToWork.ReadyToWorkType);
            Assert.AreEqual(ReadyToWorkType.Pull, readyToWork2.ReadyToWorkType);
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
