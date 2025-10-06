// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.Logging;
using NUnit.Framework;
using Moryx.TestTools.UnitTest;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class JobManagerTests
    {
        private JobManager _jobManager;
        private Mock<IJobDataList> _jobListMock;
        private Mock<IJobStorage> _jobStorageMock;
        private List<IProductionJobData> _jobList = new();
        private Mock<IJobScheduler> _jobSchedulerMock;
        private List<IJobData> _jobs = new();
        private Mock<IJobHandler> _handlerMock;

        [SetUp]
        public void SetUp()
        {
            _jobListMock = new Mock<IJobDataList>();
            _jobStorageMock = new Mock<IJobStorage>();

            _jobStorageMock.Setup(j => j.GetAll()).Returns(() => Array.Empty<IJobData>());
            _jobListMock.Setup(j => j.GetEnumerator()).Returns(() => _jobList.GetEnumerator());
            _jobListMock.As<IEnumerable>().Setup(j => j.GetEnumerator()).Returns(() => _jobList.GetEnumerator());

            _jobSchedulerMock = new Mock<IJobScheduler>();

            _handlerMock = new Mock<IJobHandler>();

            _jobManager = new JobManager
            {
                JobStorage = _jobStorageMock.Object,
                JobDataFactory = new Mock<IJobDataFactory>().Object,
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
}

