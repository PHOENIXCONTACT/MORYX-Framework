// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Tasks;
using Moryx.Model.Repositories;
using Moryx.Workplans;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessArchiveTests
    {
        private ProcessArchive _processArchive;

        private Mock<IJobEntityRepository> _jobRepo;
        private Mock<IProcessEntityRepository> _processRepo;
        private Mock<IWorkplan> _workplanMock;
        private Mock<IUnitOfWork<ProcessContext>> _uow;

        private Mock<IJobDataList> _jobListMock;

        private readonly CultureInfo _culture = new("de-DE");

        [SetUp]
        public void Setup()
        {
            _workplanMock = new Mock<IWorkplan>();
            _workplanMock.Setup(w => w.Steps).Returns(
            [
                new MountTask{Id = 1}
            ]);

            var recipe = new ProductionRecipe
            {
                Workplan = _workplanMock.Object,
            };

            var productManagementMock = new Mock<IProductManagement>();
            productManagementMock.Setup(p => p.LoadRecipe(It.IsAny<long>())).Returns(recipe);
            productManagementMock.Setup(p => p.GetInstance(It.IsAny<long>())).Returns(new DummyProductInstance { Type = new DummyProductType() });

            _jobListMock = new Mock<IJobDataList>();

            var activityPoolMock = new Mock<IActivityDataPool>();

            _jobRepo = new Mock<IJobEntityRepository>();
            _processRepo = new Mock<IProcessEntityRepository>();

            var tracingRepo = new Mock<ITracingTypeRepository>();
            tracingRepo.Setup(t => t.GetAll()).Returns(new List<TracingType>());

            var activityRepo = new Mock<IActivityEntityRepository>();
            activityRepo.Setup(a => a.GetRunning(It.IsAny<long>())).Returns(new List<ActivityEntity>());
            activityRepo.Setup(a => a.GetCompleted(It.IsAny<long>())).Returns(new List<ActivityEntity>());

            _uow = new Mock<IUnitOfWork<ProcessContext>>();
            _uow.Setup(u => u.GetRepository<IJobEntityRepository>()).Returns(_jobRepo.Object);
            _uow.Setup(u => u.GetRepository<IProcessEntityRepository>()).Returns(_processRepo.Object);
            _uow.Setup(u => u.GetRepository<ITracingTypeRepository>()).Returns(tracingRepo.Object);
            _uow.Setup(u => u.GetRepository<IActivityEntityRepository>()).Returns(activityRepo.Object);

            var uowFactoryMock = new Mock<IUnitOfWorkFactory<ProcessContext>>();
            uowFactoryMock.Setup(u => u.Create()).Returns(_uow.Object);
            uowFactoryMock.Setup(u => u.Create()).Returns(_uow.Object);

            var processStorage = new ProcessStorage { UnitOfWorkFactory = uowFactoryMock.Object };

            _processArchive = new ProcessArchive
            {
                ProductManagement = productManagementMock.Object,
                JobList = _jobListMock.Object,
                UnitOfWorkFactory = uowFactoryMock.Object,
                ActivityPool = activityPoolMock.Object,
                ProcessStorage = processStorage
            };

            _processArchive.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _processArchive.Dispose();
        }

        [Test]
        public void GetProcessesByInstance()
        {
            // Arrange
            var jobs = CreateJobEntities();
            CreateTestData(jobs, CreateProcessEntities(jobs));

            // Act
            var processes = _processArchive.GetProcesses(new DummyProductInstance { Id = 42 });

            // Assert
            Assert.That(processes.Count, Is.EqualTo(1));
        }

        [TestCase("1.1.1900 00:00:00", "1.1.2100 00:00:00", 4, 2, 1, Description = "Get all Processed")]
        [TestCase("1.1.2000 00:00:00", "1.1.2000 00:00:01", 0, 0, 0, Description = "Get no processes")]
        [TestCase("1.1.2000 01:15:00", "1.1.2000 01:20:00", 1, 1, 0, Description = "Get 1 Process")]
        [TestCase("1.1.2000 01:10:01", "1.1.2000 01:40:00", 3, 1, 0, Description = "Get 3 Processes")]
        public void GetProcessesTest(string start, string end,
                                     int expectedJobCount,
                                     int expectedProcessCountJob1,
                                     int expectedProcessCountJob2)
        {
            // Arrange
            var jobs = CreateJobEntities();
            CreateTestData(jobs, CreateProcessEntities(jobs));

            var startDate = DateTime.Parse(start, _culture);
            var endDate = DateTime.Parse(end, _culture);

            // Act
            var chunks = _processArchive.GetProcesses(RequestFilter.Timed, startDate, endDate, []).ToList();

            // Assert
            Assert.That(chunks.Count, Is.EqualTo(expectedJobCount));
            if (chunks.Count > 0)
                Assert.That(chunks[0].Count(), Is.EqualTo(expectedProcessCountJob1));

            if (chunks.Count > 1)
                Assert.That(chunks[1].Count(), Is.EqualTo(expectedProcessCountJob2));

            if (chunks.Count > 2)
                Assert.That(chunks[2].Count(), Is.EqualTo(0));
            if (chunks.Count > 3)
                Assert.That(chunks[3].Count(), Is.EqualTo(0));
        }

        [Test]
        public void AddingNotCompletedJobToCacheTest()
        {
            // Arrange
            var jobs = CreateJobEntities();
            CreateTestData(jobs, CreateProcessEntities(jobs));

            // Act
            _jobListMock.Raise(j => j.StateChanged += null, _jobListMock.Object,
                                new ProductionJobData(new DummyRecipe(),
                                new JobEntity
                                {
                                    Id = 10,
                                    Amount = 10,
                                    Created = new DateTime(2000, 1, 1, 1, 50, 0),
                                    Updated = new DateTime(2000, 1, 1, 1, 59, 0)
                                }));

            var chunks = _processArchive.GetProcesses(RequestFilter.Timed, new DateTime(2000, 1, 1, 1, 50, 0), new DateTime(2000, 1, 1, 1, 59, 0), []).ToList();

            // Assert
            Assert.That(chunks.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddingCompletedJobToCacheTest()
        {
            var now = DateTime.Now;
            var nowStart = now;
            var nowEnd = now.AddMinutes(1);

            var processStart = now.AddSeconds(1);
            var processEnd = now.AddSeconds(2);

            // Arrange
            var jobEntity = new JobEntity
            {
                Id = 10,
                Amount = 10,
                Created = nowStart,
                Updated = nowEnd
            };
            var processEntity = new ProcessEntity
            {
                Id = 10,
                JobId = jobEntity.Id,
                Job = jobEntity,
                ReferenceId = 1,
                Activities =
                    new List<ActivityEntity>
                    {
                        new() {
                            Started = processStart,
                            Completed = processEnd
                        }
                    },
                State = (int)ProcessState.Success
            };

            var processData = new ProcessData(new Process { Id = 10, Recipe = null });
            processData.AddActivity(new ActivityData(new DummyActivity { Tracing = new Tracing { Started = processStart, Completed = null } }));

            var jobData = new ProductionJobData(new DummyRecipe(), jobEntity);
            jobData.RunningProcesses.Add(processData);

            var jobs = CreateJobEntities().Concat([jobEntity]).ToArray();
            var processEntities = CreateProcessEntities(jobs).Concat([processEntity]).ToArray();
            CreateTestData(jobs, processEntities);

            // Act
            _jobListMock.Raise(j => j.StateChanged += null, _jobListMock.Object, jobData);

            var chunks = _processArchive.GetProcesses(RequestFilter.Timed, nowStart, nowEnd, []).ToList();

            // Assert
            Assert.That(chunks.Count, Is.EqualTo(1));
            Assert.That(chunks[0].Count(), Is.EqualTo(1));

            processData.Activities[0].Activity.Tracing.Completed = processEnd;

            var stateMock = new Mock<IJobState>();
            stateMock.SetupGet(s => s.Classification).Returns(JobClassification.Completed);
            _jobListMock.Raise(jl => jl.StateChanged += null, _jobListMock.Object, new JobStateEventArgs(jobData, null, stateMock.Object));

            chunks = _processArchive.GetProcesses(RequestFilter.Timed, nowStart, nowEnd, []).ToList();

            // Assert
            Assert.That(chunks.Count, Is.EqualTo(1));
            Assert.That(chunks[0].Count(), Is.EqualTo(1));
            var process = (ProductionProcess)chunks[0].First();
            Assert.That(process.ProductInstance, Is.InstanceOf<DummyProductInstance>());
        }

        private void CreateTestData(IEnumerable<JobEntity> jobs, IEnumerable<ProcessEntity> processes)
        {
            var queryableJobEntityMock = new Mock<IQueryable<JobEntity>>();
            SetupIQueryable(queryableJobEntityMock, jobs.AsQueryable());
            _jobRepo.Setup(j => j.Linq).Returns(queryableJobEntityMock.Object);

            var queryableProcessEntityMock = new Mock<IQueryable<ProcessEntity>>();
            SetupIQueryable(queryableProcessEntityMock, processes.AsQueryable());
            _processRepo.Setup(j => j.Linq).Returns(queryableProcessEntityMock.Object);
        }

        private JobEntity[] CreateJobEntities()
        {
            return
            [
                new JobEntity { Id = 1, Amount = 10, Created = new DateTime(2000, 1, 1, 1, 0, 0), Updated = new DateTime(2000, 1, 1, 1, 10, 0)},
                new JobEntity { Id = 2, Amount = 10, Created = new DateTime(2000, 1, 1, 1, 15, 0), Updated = new DateTime(2000, 1, 1, 1, 20, 0)},
                new JobEntity { Id = 3, Amount = 10, Created = new DateTime(2000, 1, 1, 1, 30, 0), Updated = new DateTime(2000, 1, 1, 1, 40, 0)},
                new JobEntity { Id = 4, Amount = 10, Created = new DateTime(2000, 1, 1, 1, 25, 0), Updated = new DateTime(2000, 1, 1, 1, 40, 0)},
            ];
        }

        private ProcessEntity[] CreateProcessEntities(JobEntity[] jobs)
        {
            return
            [
                // Job 1
                new ProcessEntity { JobId = jobs[0].Id, Job = jobs[0],
                    Activities = new List<ActivityEntity> { new() { Started = new DateTime(2000, 1, 1, 1, 0, 1), Completed = new DateTime(2000, 1, 1, 1, 0, 2) } }, State = (int)ProcessState.Success },
                new ProcessEntity { Id = 1337, JobId = jobs[0].Id, Job = jobs[0], ReferenceId = 42,
                    Activities = new List<ActivityEntity>
                    {
                        new() {
                            TaskId = 1,
                            Started = new DateTime(2000, 1, 1, 1, 0, 3),
                            Completed = new DateTime(2000, 1, 1, 1, 0, 9)
                        }
                    }, State = (int)ProcessState.Success },

                // Job 2
                new ProcessEntity { JobId = jobs[1].Id, Job = jobs[1], Activities = new List<ActivityEntity> { new() { Started = new DateTime(2000, 1, 1, 1, 15, 1), Completed = new DateTime(2000, 1, 1, 1, 15, 2) } }, State = (int)ProcessState.Failure }
            ];
        }

        public static void SetupIQueryable<T>(Mock<T> mock, IQueryable queryable)
            where T : class, IQueryable
        {
            mock.Setup(r => r.GetEnumerator()).Returns(queryable.GetEnumerator());
            mock.Setup(r => r.Provider).Returns(queryable.Provider);
            mock.Setup(r => r.ElementType).Returns(queryable.ElementType);
            mock.Setup(r => r.Expression).Returns(queryable.Expression);
        }
    }
}

