// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes;

[TestFixture]
public class ProcessRemovalTests : ProcessTestsBase
{
    private ProcessRemoval _removal;

    private Mock<IResourceManagement> _resourceManagementMock;
    private Mock<IProcessReporter> _removalResourceMock;
    private Mock<ICell> _productionCellMock;

    [SetUp]
    public void CreateProcessRemoval()
    {
        CreateList();

        _resourceManagementMock = new Mock<IResourceManagement>();
        _removalResourceMock = new Mock<IProcessReporter>();
        _productionCellMock = CreateProductionCell(_resourceManagementMock);

        _resourceManagementMock.Setup(rm => rm.GetResources<IProcessReporter>())
            .Returns(() => [_removalResourceMock.Object]);

        var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
        _removal = new ProcessRemoval
        {
            ActivityPool = DataPool,
            Config = new ModuleConfig
            {
                RemovalMessage = "FUUUU!"
            },
            Logger = logger,
            ResourceManagement = _resourceManagementMock.Object
        };
        _removal.Initialize();
        _removal.Start();
    }

    [TearDown]
    public void DestroyProcessRemoval()
    {
        _removal.Dispose();
        DestroyList();
    }

    [TestCase(true, Description = "Wait for completed activity to set RemoveBroken")]
    [TestCase(false, Description = "Directly set RemoveBroken for process without activity")]
    public void AwaitStableStateForAborting(bool currentlyRunningActivity)
    {
        // Arrange
        var process = Process(false);
        DataPool.AddProcess(process);
        if (currentlyRunningActivity)
            DataPool.AddActivity(process, new ActivityData(new AssignIdentityActivity { Process = process.Process, Id = 1 })
            {
                State = ActivityState.Running
            });

        // Act
        DataPool.UpdateProcess(process, ProcessState.Aborting);

        // Assert
        if (currentlyRunningActivity)
        {
            var activityData = process.Activities[1];
            DataPool.UpdateActivity(activityData, ActivityState.Completed);
        }

        // Assert
        Assert.That(ModifiedProcess, Is.Not.Null);
        Assert.That(ModifiedProcess.State, Is.EqualTo(ProcessState.Failure));
    }

    [TestCase(true, Description = "Provide unmount for mounted, broken process")]
    [TestCase(false, Description = "Do not provide unmount for non-mounted, broken process")]
    public void ProvideFixUpActivity(bool previouslyMounted)
    {
        // Arrange
        var process = Process(previouslyMounted);
        DataPool.AddProcess(process);

        // Act
        DataPool.UpdateProcess(process, ProcessState.RemoveBroken);

        // Assert
        if (previouslyMounted)
        {
            Assert.That(ModifiedActivity, Is.Not.Null);
            Assert.That(ModifiedActivity.Activity, Is.InstanceOf<ProcessFixupActivity>());
        }
        else
        {
            Assert.That(ModifiedActivity, Is.Null);
            Assert.That(ModifiedProcess.State, Is.EqualTo(ProcessState.Failure));
            return;
        }

        // Complete activity
        ModifiedActivity.Activity.Complete(0);
        DataPool.UpdateActivity(ModifiedActivity, ActivityState.ResultReceived);

        // Assert
        Assert.That(ModifiedProcess, Is.Not.Null);
        Assert.That(ModifiedProcess.State, Is.EqualTo(ProcessState.Failure));
    }

    [Test(Description = "Check that the dispatcher removes all Activities from the pool and marks the process as failed, when a ProcessRemoved occures")]
    public void CheckThatProcessWasRemovedMessagesAreProcessed()
    {
        // Arrange

        //Make sure there are Activities in the pool
        var activityData = FillPool(new MountActivity(), _productionCellMock.Object);
        DataPool.UpdateActivity(activityData, ActivityState.Configured);

        // Act
        _removalResourceMock.Raise(r => r.ProcessRemoved += null, _removalResourceMock.Object, activityData.ProcessData.Process);

        // Assert
        Assert.That(activityData.ProcessData.State, Is.EqualTo(ProcessState.Failure), "Process should be marked as a failure.");
    }

    [Test(Description = "Check that the dispatcher removes all Activities from the pool and marks the process as failed, when a ProcessRemoved occures")]
    public void CheckProcessBrokenAbortsProcess()
    {
        // Arrange

        //Make sure there are Activities in the pool
        var activityData = FillPool(new MountActivity(), _productionCellMock.Object);
        DataPool.UpdateActivity(activityData, ActivityState.Configured);

        // Act
        _removalResourceMock.Raise(r => r.ProcessBroken += null, _removalResourceMock.Object, activityData.ProcessData.Process);

        // Assert
        Assert.That(activityData.ProcessData.State, Is.EqualTo(ProcessState.Aborting), "Process should be marked as a aborting.");
    }

    private static ProcessData Process(bool mounted)
    {
        var recipe = new ProductionRecipe();
        var process = new ProcessData(new Process())
        {
            Job = new ProductionJobData(recipe, 0),
            Recipe = new ProductRecipe()
        };
        if (mounted)
            process.AddActivity(new ActivityData(new MountActivity())
            {
                Result = new ActivityResult
                {
                    Numeric = (int)MountingResult.Mounted,
                },
                State = ActivityState.Completed
            });
        process.AddActivity(new ActivityData(new DummyActivity())
        {
            Result = new ActivityResult
            {
                Numeric = (int)DefaultActivityResult.Success,
            },
            State = ActivityState.Completed
        });
        return process;
    }
}