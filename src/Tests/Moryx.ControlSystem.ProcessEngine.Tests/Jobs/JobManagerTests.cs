// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.Logging;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs;

[TestFixture]
public class JobManagerTests
{
    private JobManager _jobManager;
    private Mock<IJobDataList> _jobListMock;
    private Mock<IJobStorage> _jobStorageMock;
    private readonly List<IProductionJobData> _jobList = new();
    private Mock<IJobScheduler> _jobSchedulerMock;
    private Mock<IJobHandler> _handlerMock;
    private Mock<IJobDataFactory> _jobDataFactoryMock;

    [SetUp]
    public void SetUp()
    {
        _jobListMock = new Mock<IJobDataList>();
        _jobStorageMock = new Mock<IJobStorage>();

        _jobStorageMock.Setup(j => j.GetAll()).Returns(Array.Empty<IJobData>);
        _jobListMock.Setup(j => j.GetEnumerator()).Returns(() => _jobList.GetEnumerator());
        _jobListMock.As<IEnumerable>().Setup(j => j.GetEnumerator()).Returns(() => _jobList.GetEnumerator());

        _jobSchedulerMock = new Mock<IJobScheduler>();

        _handlerMock = new Mock<IJobHandler>();
        _jobDataFactoryMock = new Mock<IJobDataFactory>();

        _jobManager = new JobManager
        {
            JobStorage = _jobStorageMock.Object,
            JobDataFactory = _jobDataFactoryMock.Object,
            JobList = _jobListMock.Object,
            JobHandlers = [_handlerMock.Object],
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
            ParallelOperations = new NotSoParallelOps()
        };
        _jobManager.Configure(_jobSchedulerMock.Object);

        _jobManager.Start();
    }

    [TearDown]
    public void Cleanup()
    {
        _jobList.Clear();
    }

    [Test(Description = "If the scheduler changes the order of jobs, it is corrected by the JobManager")]
    public void JobManagerPreservesJobOrderInScheduler()
    {
        // Arrange
        IList<IJobData> expanded = null;
        var jobs = new[] { CreateJobMock(1), CreateJobMock(2) };
        _jobListMock.Setup(j => j.Add(It.IsAny<LinkedList<IJobData>>(), JobPosition.Expand, It.IsAny<Action<ModifiedJobsFragment>>()))
            .Callback((LinkedList<IJobData> jobDatas, JobPosition position, Action<ModifiedJobsFragment> callback) => expanded = jobDatas.ToList());
        _jobListMock.Setup(j => j.Next(jobs[0])).Returns(jobs[1]);
        _jobSchedulerMock.Setup(j => j.SchedulableJobs(It.IsAny<IEnumerable<Job>>()))
            .Returns<IEnumerable<Job>>(j => j.Reverse());

        _jobList.AddRange(jobs);

        // Act
        _jobSchedulerMock.Raise(j => j.SlotAvailable += null, EventArgs.Empty);

        // Assert
        Assert.That(expanded, Is.Not.Null);
        Assert.That(expanded.Count, Is.EqualTo(2));
        Assert.That(expanded[0].Id, Is.EqualTo(1));
        Assert.That(expanded[1].Id, Is.EqualTo(2));
    }

    [Test(Description = "If there are gaps within the schedulable jobs, we need separate chunks to place jobs in the list")]
    public void SplitSchedulableGapIntoChunks()
    {
        // Arrange
        var chunks = new List<LinkedList<IJobData>>();
        var jobs = new[] { CreateJobMock(1), CreateJobMock(2), CreateJobMock(3), CreateJobMock(4) };
        _jobListMock.Setup(j => j.Add(It.IsAny<LinkedList<IJobData>>(), JobPosition.Expand, It.IsAny<Action<ModifiedJobsFragment>>()))
            .Callback((LinkedList<IJobData> jobDatas, JobPosition position, Action<ModifiedJobsFragment> callback) => chunks.Add(jobDatas));
        _jobListMock.Setup(j => j.Next(jobs[0])).Returns(jobs[1]);
        _jobListMock.Setup(j => j.Next(jobs[1])).Returns(jobs[2]);
        _jobListMock.Setup(j => j.Next(jobs[2])).Returns(jobs[3]);
        _jobSchedulerMock.Setup(j => j.SchedulableJobs(It.IsAny<IEnumerable<Job>>()))
            .Returns<IEnumerable<Job>>(j => [jobs[0].Job, jobs[2].Job, jobs[3].Job]);
        _handlerMock.Setup(h => h.Handle(It.IsAny<LinkedList<IJobData>>()))
            .Callback((LinkedList<IJobData> j) => { j.AddFirst(CreateJobMock()); j.AddLast(CreateJobMock()); });

        _jobList.AddRange(jobs);

        // Act
        _jobSchedulerMock.Raise(j => j.SlotAvailable += null, EventArgs.Empty);

        // Assert
        Assert.That(chunks.Count, Is.EqualTo(2));
        Assert.That(chunks[0].ElementAt(1).Id, Is.EqualTo(1));
        Assert.That(chunks[1].ElementAt(1).Id, Is.EqualTo(3));
        Assert.That(chunks[1].ElementAt(2).Id, Is.EqualTo(4));
    }

    [Test(Description = "If a job is reported as completed the job manager triggers removal")]
    public void JobManagerRemovesCompletedJob()
    {
        // Arrange
        _jobList.AddRange([CreateJobMock(1), CreateJobMock(2), CreateJobMock(3)]);

        // Act
        var completed = _jobList[1];
        var stateMock = new Mock<IJobState>();
        stateMock.SetupGet(s => s.Classification).Returns(JobClassification.Completed);
        _jobListMock.Raise(jl => jl.StateChanged += null, _jobListMock.Object, new JobStateEventArgs(completed, null, stateMock.Object));

        // Assert
        _jobListMock.Verify(jl => jl.Remove(completed, _jobStorageMock.Object.Save));
    }

    [Test(Description = "Jobs completing during shutdown must still be removed to keep the stored job links consistent")]
    public void JobManagerRemovesCompletedJobAfterStop()
    {
        // Arrange
        _jobList.AddRange([CreateJobMock(1), CreateJobMock(2), CreateJobMock(3)]);
        _jobManager.Stop();

        // Act
        var completed = _jobList[1];
        RaiseStateChanged(completed, JobClassification.Running, JobClassification.Completed);

        // Assert
        _jobListMock.Verify(jl => jl.Remove(completed, _jobStorageMock.Object.Save), Times.Once);
        _jobDataFactoryMock.Verify(f => f.Destroy(completed), Times.Once);
    }

    [Test(Description = "Completed jobs are not forwarded to the already stopped scheduler during shutdown")]
    public void JobManagerDoesNotForwardCompletionToSchedulerAfterStop()
    {
        // Arrange
        _jobList.AddRange([CreateJobMock(1)]);
        _jobManager.Stop();

        // Act
        var completed = _jobList[0];
        RaiseStateChanged(completed, JobClassification.Running, JobClassification.Completed);

        // Assert
        _jobSchedulerMock.Verify(s => s.JobUpdated(It.IsAny<Job>(), It.IsAny<JobClassification>()), Times.Never);
        _jobListMock.Verify(jl => jl.Remove(completed, _jobStorageMock.Object.Save), Times.Once);
    }

    [Test(Description = "Intermediate state changes must not remove the job from the job list")]
    public void JobManagerDoesNotRemoveJobOnIntermediateStateChange()
    {
        // Arrange
        _jobList.AddRange([CreateJobMock(1)]);

        // Act
        var running = _jobList[0];
        RaiseStateChanged(running, JobClassification.Waiting, JobClassification.Running);

        // Assert
        _jobSchedulerMock.Verify(s => s.JobUpdated(running.Job, JobClassification.Running), Times.Once);
        _jobListMock.Verify(jl => jl.Remove(It.IsAny<IJobData>(), It.IsAny<Action<ModifiedJobsFragment>>()), Times.Never);
        _jobDataFactoryMock.Verify(f => f.Destroy(It.IsAny<IJobData>()), Times.Never);
    }

    private void RaiseStateChanged(IJobData jobData, JobClassification previous, JobClassification current)
    {
        var previousStateMock = new Mock<IJobState>();
        previousStateMock.SetupGet(s => s.Classification).Returns(previous);
        var currentStateMock = new Mock<IJobState>();
        currentStateMock.SetupGet(s => s.Classification).Returns(current);

        _jobListMock.Raise(jl => jl.StateChanged += null, _jobListMock.Object,
            new JobStateEventArgs(jobData, previousStateMock.Object, currentStateMock.Object));
    }

    private static IProductionJobData CreateJobMock(int id = 0)
    {
        var job = new EngineJob(new ProductRecipe { Id = id + 20 }, 1)
        {
            Id = id,
            Classification = JobClassification.Idle
        };

        var jobMock = new Mock<IProductionJobData>();
        jobMock.SetupGet(j => j.Id).Returns(job.Id);
        jobMock.SetupGet(j => j.Job).Returns(job);
        var jobStateMock = new Mock<IJobState>();
        jobStateMock.SetupGet(s => s.Classification).Returns(job.Classification);
        jobMock.SetupGet(j => j.State).Returns(jobStateMock.Object);

        return jobMock.Object;
    }

}
