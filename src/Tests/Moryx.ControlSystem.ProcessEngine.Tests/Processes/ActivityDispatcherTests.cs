// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.Logging;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes;

[TestFixture]
public class ActivityDispatcherTests : ProcessTestsBase
{
    private ActivityDispatcher _dispatcher;
    private Mock<IResourceManagement> _resourceManagementMock;
    private Mock<ICell> _productionCellMock;
    private Mock<ICell> _mountCellMock;
    private Mock<ICell> _serialCellMock;

    [SetUp]
    public void CreateDispatcher()
    {
        CreateList();

        _resourceManagementMock = new Mock<IResourceManagement>();
        _productionCellMock = CreateProductionCell(_resourceManagementMock);
        _mountCellMock = CreateMountCell(_resourceManagementMock, true, false);
        _serialCellMock = CreateSerialCell(_resourceManagementMock);

        _resourceManagementMock.Setup(rm => rm.GetResources<ICell>()).Returns(() =>
        [
            _productionCellMock.Object,
            _mountCellMock.Object,
            _serialCellMock.Object
        ]);

        _dispatcher = new ActivityDispatcher
        {
            ActivityPool = DataPool,
            ParallelOperations = new NotSoParallelOps(),
            ResourceManagement = _resourceManagementMock.Object,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        _dispatcher.Initialize();
        _dispatcher.Start();
    }

    [TearDown]
    public void DestoryDispatcher()
    {
        _dispatcher.Dispose();
        _resourceManagementMock = null;
        DestroyList();
    }

    [Test(Description = "Dispatch dummy activity to cell 1")]
    public void DispatchActivityOnReadyToWorkPull()
    {
        // Arrange
        var dummy = new DummyActivity();
        var activity = FillPool(dummy, _productionCellMock.Object);

        // Act
        DataPool.UpdateActivity(activity, ActivityState.Configured);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, ValidProcessId);

        RaiseReadyToWork(_productionCellMock, rtw);

        // Assert
        AssertActivityDispatch(dummy, _productionCellMock);
        Assert.That(activity.ProcessData.State, Is.EqualTo(ProcessState.Running));
    }

    [Test(Description = "Dispatch assign identity to push rtw in cell 3")]
    public void DispatchActivityOnReadtyToWorkPush()
    {
        // Arrange
        var dummy = new AssignIdentityActivity();
        var activity = FillPool(dummy, _serialCellMock.Object);

        // Act
        DataPool.UpdateActivity(activity, ActivityState.Configured);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push, ValidProcessId);

        RaiseReadyToWork(_serialCellMock, rtw);

        // Assert
        AssertActivityDispatch(dummy, _serialCellMock);
    }

    [Test]
    public void DispatchActivityToNewCell()
    {
        // Arrange
        var unmountMock = CreateMountCell(_resourceManagementMock, false, true);
        unmountMock.SetupGet(r => r.Id).Returns(NewCellId);
        var dummy = new UnmountActivity();
        var activity = FillPool(dummy, unmountMock.Object);

        // Act
        DataPool.UpdateActivity(activity, ActivityState.Configured);
        _resourceManagementMock.Raise(rm => rm.ResourceAdded += null,
            _resourceManagementMock.Object, unmountMock.Object);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push, ValidProcessId);

        RaiseReadyToWork(unmountMock, rtw);

        // Assert
        AssertActivityDispatch(dummy, unmountMock);
    }

    [Test(Description = "The list of reported sessions should be adjusted for the removed cell and the cell should be notified of the detachement")]
    public void RemoveReportedSessionWhenCellIsRemoved()
    {
        // Arrange
        var unmountMock = CreateMountCell(_resourceManagementMock, false, true);
        unmountMock.SetupGet(r => r.Id).Returns(NewCellId);

        // Act
        // Add a session to the dispatcher
        _resourceManagementMock.Raise(rm => rm.ResourceAdded += null,
            _resourceManagementMock.Object, unmountMock.Object);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push, 0);

        RaiseReadyToWork(unmountMock, rtw);

        // The session should be stored in the list of reported sessions
        Assert.That(_dispatcher.ExportSessions().Length, Is.EqualTo(1));

        // Remove the resource
        _resourceManagementMock.Raise(rm => rm.ResourceRemoved += null,
            _resourceManagementMock.Object, unmountMock.Object);

        // Assert
        Assert.That(_dispatcher.ExportSessions().Length, Is.EqualTo(0));
    }

    [Test(Description = "A ReadyToWork of type Push is enqueued and the activity dispatched when it changes to ConfiguredSaved")]
    public void EnqueAndDispatchPush()
    {
        // Arrange
        var dummy = new AssignIdentityActivity();
        var activity = FillPool(dummy, _serialCellMock.Object);

        // Act: First the ReadyToWork and then the activity change
        RaiseReadyToWork(_serialCellMock, Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push));
        DataPool.UpdateActivity(activity, ActivityState.Configured);

        // Assert
        AssertActivityDispatch(dummy, _serialCellMock);
    }

    private void AssertActivityDispatch(Activity dummy, Mock<ICell> resourceMock)
    {
        Assert.DoesNotThrow(delegate
        {
            resourceMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Once,
                "There should be one StartActivity call at the resource");
            resourceMock.Verify(r => r.StartActivity(It.Is<ActivityStart>(a => a.Activity == dummy)), Times.Once,
                "It was not the expected activity in the StartActivity call at the resource");
        });

        Assert.That(ModifiedActivity, Is.Not.Null);
        Assert.That(ModifiedActivity.State, Is.EqualTo(ActivityState.Running));
    }

    /// <summary>
    /// Test cases that will result in a SequenceCompleted response by the ActivityDispatcher
    /// </summary>
    public enum SequenceCompletedReason
    {
        /// <summary>
        /// No activity dispatched because the pool is empty
        /// </summary>
        EmptyPool,
        /// <summary>
        /// Process is present, but does not match ReadyToWorkProcess
        /// </summary>
        ProcessMismatch,
        /// <summary>
        /// There are activities in the pool, but the require a different resource
        /// </summary>
        ResourceMismatch,
        /// <summary>
        /// Resource is in the wrong mode
        /// </summary>
        ModeMismatch,
        /// <summary>
        /// Activity article requirement and
        /// </summary>
        RequirementMismatch,
    }
    [TestCase(SequenceCompletedReason.EmptyPool, ReadyToWorkType.Pull, false, Description = "ActivityDispatcher sends SequenceCompleted because pool is empty")]
    [TestCase(SequenceCompletedReason.EmptyPool, ReadyToWorkType.Pull, true, Description = "ActivityDispatcher sends SequenceCompleted because pool is empty")]
    [TestCase(SequenceCompletedReason.ProcessMismatch, ReadyToWorkType.Pull, true, Description = "ActivityDispatcher sends SequenceCompleted because process id does not match")]
    [TestCase(SequenceCompletedReason.ProcessMismatch, ReadyToWorkType.Push, true, Description = "ActivityDispatcher sends SequenceCompleted for RTW.Push because process id does not match")]
    [TestCase(SequenceCompletedReason.ResourceMismatch, ReadyToWorkType.Pull, false, Description = "ActivityDispatcher sends SequenceCompleted because process id does not match")]
    [TestCase(SequenceCompletedReason.ResourceMismatch, ReadyToWorkType.Pull, true, Description = "ActivityDispatcher sends SequenceCompleted because process id does not match")]
    [TestCase(SequenceCompletedReason.ModeMismatch, ReadyToWorkType.Pull, true, Description = "ActivityDispatcher sends SequenceCompleted because the mode does not match")]
    [TestCase(SequenceCompletedReason.RequirementMismatch, ReadyToWorkType.Pull, true, Description = "ActivityDispatcher sends SequenceCompleted because mount requires empty carrier")]
    [TestCase(SequenceCompletedReason.RequirementMismatch, ReadyToWorkType.Pull, false, Description = "ActivityDispatcher sends SequenceCompleted because dummy requires an article")]
    public void SendSequenceCompleted(SequenceCompletedReason reason, ReadyToWorkType rtwType, bool processPresent)
    {
        // Arrange
        Mock<ICell> resourceMock = _mountCellMock;
        switch (reason)
        {
            case SequenceCompletedReason.EmptyPool:
                // Empty means empty!
                break;
            default:
                var activity = reason == SequenceCompletedReason.RequirementMismatch & processPresent
                    ? (Activity)new MountActivity()
                    : new DummyActivity();
                var possibleResource = reason != SequenceCompletedReason.ResourceMismatch ? _mountCellMock : _productionCellMock;
                var activityData = FillPool(activity, possibleResource.Object);
                resourceMock = reason == SequenceCompletedReason.ResourceMismatch ? _mountCellMock : _productionCellMock;
                DataPool.UpdateActivity(activityData, ActivityState.Configured);
                break;
        }

        // Act
        ReadyToWork rtw;
        switch (reason)
        {
            case SequenceCompletedReason.ProcessMismatch:
                rtw = Session.StartSession(ActivityClassification.Production, rtwType, InvalidProcessId);
                break;
            case SequenceCompletedReason.ModeMismatch:
                rtw = Session.StartSession(ActivityClassification.Maintenance, rtwType, ValidProcessId);
                break;
            default:
                rtw = processPresent
                    ? Session.StartSession(ActivityClassification.Production, rtwType, ValidProcessId)
                    : Session.StartSession(ActivityClassification.Production, rtwType);
                break;
        }
        RaiseReadyToWork(resourceMock, rtw);

        // Assert
        Assert.DoesNotThrow(() => resourceMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once));
    }

    [Test(Description = "Send SequenceCompleted for RTW.Push if the process was removed.")]
    public void SendSequenceCompletedOnProcessRemoval()
    {
        // Arrange
        var process = new ProcessData(new ProductionProcess() { Id = ValidProcessId }) { State = ProcessState.Running };
        DataPool.AddProcess(process);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push, ValidProcessId);
        RaiseReadyToWork(_serialCellMock, rtw);

        // Act
        DataPool.UpdateProcess(process, ProcessState.Success);

        // Assert
        Assert.DoesNotThrow(() => _serialCellMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once));
    }

    [TestCase(true, Description = "Remove a ReadyToWork that exisits")]
    [TestCase(false, Description = "Remove a ReadyToWork that does not exisit")]
    public void RemoveReadyToWork(bool present)
    {
        // Arrange
        var session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
        if (present)
        {
            RaiseReadyToWork(_serialCellMock, session);
        }
        var activityData = FillPool(new AssignIdentityActivity(), _serialCellMock.Object);

        // Act: Remove rtw and make activity available
        RaiseNotReadyToWork(_serialCellMock, session.PauseSession());
        DataPool.UpdateActivity(activityData, ActivityState.Configured);

        // Assert: Make sure the activity was not dispatched
        Assert.DoesNotThrow(() => _serialCellMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Never),
            "The activty should be not dispatched and no StartActivity call at the SerialCell should be occured");
    }

    [Test]
    public void UpdateActivityOnCompletion()
    {
        // Arrange: Running activity in pool
        var dummy = new DummyActivity();
        var activityData = FillPool(dummy, _productionCellMock.Object);
        DataPool.UpdateActivity(activityData, ActivityState.Configured);
        DataPool.UpdateActivity(activityData, ActivityState.Running);

        var activityStart = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, ValidProcessId).StartActivity(dummy);

        // Act: Complete the activity
        dummy.Complete((int)DummyResult.Done);
        var completed = activityStart.CreateResult();
        RaiseActivityCompleted(_productionCellMock, completed);

        // Assert: Make sure activity data was updated to running
        Assert.That(ModifiedActivity, Is.EqualTo(activityData));
        Assert.That(ModifiedActivity.State, Is.EqualTo(ActivityState.ResultReceived));
    }

    [Test]
    public void ReadyToWorkOverride()
    {
        // Arrange: Place a push message in the queue
        var first = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
        RaiseReadyToWork(_serialCellMock, first);
        var activityData = FillPool(new AssignIdentityActivity(), _serialCellMock.Object);

        // Act: Override rtw and update activity
        var second = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
        RaiseReadyToWork(_serialCellMock, second);
        activityData.Targets = [_serialCellMock.Object];
        DataPool.UpdateActivity(activityData, ActivityState.Configured);

        // Assert: Make sure activity was started on second session
        Assert.DoesNotThrow(delegate
        {
            _serialCellMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Once,
                "There should be only one StartActivity call at the SerialCell");
            _serialCellMock.Verify(r => r.StartActivity(It.Is<ActivityStart>(a => a.Id == second.Id)),
                "There should be a StartActivity call at the SerialCell with the id of the second ReadyToWork");
        });
    }

    public static IEnumerable<dynamic> CachedRTWTestCases()
    {
        var testCase1 = new TestCaseData(ReadyToWorkType.Push, null, ProcessState.Initial);
        testCase1.SetDescription("Raising a RTW Push without a ProcessReference, the ProcessState is arbitrary.");
        yield return testCase1;

        var testCase2 = new TestCaseData(ReadyToWorkType.Push, InvalidProcessId, ProcessState.Initial);
        testCase2.SetDescription("Raising an RTW Push, with a ProcessId that exists but does not have an Activity waiting in the Pool, " +
                                 "the ProcessState is arbitrary.");
        yield return testCase2;

        var testCase3 = new TestCaseData(ReadyToWorkType.Pull, InvalidProcessId, ProcessState.CleaningUpReady);
        testCase3.SetDescription("Raising an RTW Pull, with a ProcessId that exists but does not have an Activity waiting in the Pool. " +
                                 "The existing process hasn't reached EngineStarted state yet.");
        yield return testCase3;

        var testCase4 = new TestCaseData(ReadyToWorkType.Pull, InvalidProcessId, ProcessState.Stopping);
        testCase4.SetDescription("Raising an RTW Pull, with a ProcessId that exists but does not have an Activity waiting in the Pool. " +
                                 "The existing process is already stopping.");
        yield return testCase4;
    }
    [Test, TestCaseSource(nameof(CachedRTWTestCases))]
    public void ShouldCacheRTWWithoutDirectResponse(ReadyToWorkType rtwType, long? processId = null, ProcessState processState = ProcessState.Initial)
    {
        // Arrange
        var dummy = new AssignIdentityActivity();
        var activity = FillPool(dummy, _serialCellMock.Object);
        var process = new ProductionProcess { Id = InvalidProcessId };
        var processData = new ProcessData(process);
        DataPool.AddProcess(processData);
        DataPool.UpdateProcess(processData, processState);
        ReadyToWork readyToWork;
        if (processId is null)
            readyToWork = Session.StartSession(ActivityClassification.Production, rtwType);
        else
            readyToWork = Session.StartSession(ActivityClassification.Production, rtwType, processId.Value);

        // Act
        RaiseReadyToWork(_serialCellMock, readyToWork);

        // Assert
        _serialCellMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Never,
            "There should be no StartActivity call at the resource");

        _serialCellMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Never,
            "There should be no SequenceCompleted call at the SerialCell.");
    }

    [Test]
    public void InformCellAboutProcessAbortion()
    {
        // Arrange
        var dummy = new AssignIdentityActivity();
        var activity = FillPool(dummy, _serialCellMock.Object);
        DataPool.UpdateActivity(activity, ActivityState.Configured);
        var readyToWork = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull);
        RaiseReadyToWork(_serialCellMock, readyToWork);

        // Act
        DataPool.UpdateProcess(activity.ProcessData, ProcessState.Aborting);

        // Assert
        Assert.DoesNotThrow(delegate
        {
            _serialCellMock.Verify(r => r.ProcessAborting(dummy), Times.Once,
                "Dispatcher should inform the cell about the aborting process");
        });
    }

    [Test(Description = "Check that the dispatcher removes all Activities from the pool and informs the cells, when a process is completed")]
    public void AbortOpenActivitiesForCompletedProcesses()
    {
        // Arrange

        //Make sure there are Activities in the pool
        var activityData = FillPool(new MountActivity(), _productionCellMock.Object);
        DataPool.UpdateActivity(activityData, ActivityState.Configured);

        // Act
        DataPool.UpdateProcess(activityData.ProcessData, ProcessState.Failure);

        // Assert
        var result = DataPool.GetAllOpen(activityData.ProcessData);
        Assert.That(result, Is.Empty, "There should be no more activities.");
    }

    [Test(Description = "Do not dispatch dummy activity to cell 1 because of constrain.")]
    public void NoActvitityDispatchingIfConstraintsNotFit()
    {
        // Arrange
        var dummy = new DummyActivity();
        var activity = FillPool(dummy, _productionCellMock.Object);

        Mock<IConstraint> constraintMock = new Mock<IConstraint>();
        constraintMock.Setup(c => c.Check(It.IsAny<IConstraintContext>())).Returns(() => false);
        var constraints = new[] { constraintMock.Object };

        // Act
        DataPool.UpdateActivity(activity, ActivityState.Configured);
        var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Pull, ValidProcessId, constraints);

        RaiseReadyToWork(_productionCellMock, rtw);

        // Assert
        Assert.DoesNotThrow(delegate
        {
            _productionCellMock.Verify(r => r.SequenceCompleted(It.IsAny<SequenceCompleted>()), Times.Once,
                "There should be a SequenceCompleted call at the ProductionCell");
            _productionCellMock.Verify(r => r.StartActivity(It.IsAny<ActivityStart>()), Times.Never,
                "There should be no StartActivity call at the ProductionCell");
        });
    }

}