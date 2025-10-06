// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.Logging;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Tools;
using NUnit.Framework;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessStorageTests
    {
        private static int _index;
        private readonly Dictionary<JobEntity, int> _jobEntities = new Dictionary<JobEntity, int>();
        private IUnitOfWorkFactory<ProcessContext> _unitOfWorkFactory;
        private ProcessStorage _storage;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            ReflectionTool.TestMode = true;
            _unitOfWorkFactory = BuildUnitOfWorkFactory();

            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            _storage = new ProcessStorage { UnitOfWorkFactory = _unitOfWorkFactory, Logger = logger };
            _storage.Start();

            CreateDatabaseItems();
        }

        protected virtual UnitOfWorkFactory<ProcessContext> BuildUnitOfWorkFactory()
        {
            // Prepare in memory ControlSystem db
            return new UnitOfWorkFactory<ProcessContext>(new InMemoryDbContextManager(nameof(ProcessStorageTests)));
        }

        #region Tests

        [Test]
        public void TestGetByJob()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                foreach (var jobEntityPair in _jobEntities)
                {
                    int count = jobEntityPair.Value;
                    var jobData = new ProductionJobData(DummyRecipe.BuildRecipe(), jobEntityPair.Key);
                    var all = new List<ProcessData>();
                    _storage.LoadCompletedProcesses(uow, jobData, all);
                    var results = _storage.GetRunningProcesses(uow, jobData);

                    //Check that GetByJob retrieved as many processes as were attached to the individual job
                    Assert.That(all.Count, Is.EqualTo(1));
                    Assert.That(((ProductionProcess)all[0].Process).ProductInstance, Is.Not.Null);
                    Assert.That(count, Is.EqualTo(results.Count), "GetByJob returned the wrong number of processes.");
                }
            }
        }

        [Test]
        public void TestLoadActivityProperties()
        {
            const int activityCount = 4;

            var task = new TaskTransition<MountActivity>(new MountingParameters(), null) { Id = 42 };

            var originalDataInstances = new Dictionary<long, ActivityData>();
            var dataInstancesForLoadProperties = new Dictionary<long, ActivityData>();

            long processId, jobId;
            ProcessData processData;
            using (var uow = _unitOfWorkFactory.Create())
            {
                var jobEntity = CreateJobEntity(uow);
                var processEntity = CreateProcessEntity(uow, nameof(ProductionProcess), (int)ProcessState.Running, 0);
                processEntity.Job = jobEntity;
                uow.SaveChanges();
                processId = processEntity.Id;
                jobId = jobEntity.Id;
                processData = CreateProcessData(jobId, processId);

                // Create [activityCount] Activities
                for (int i = 0; i <= activityCount; i++)
                {
                    var activityData = CreateActivityData(new MountActivity(), processData);
                    processData = activityData.ProcessData = processData ?? activityData.ProcessData;
                    activityData.Id = IdShiftGenerator.Generate(processId, ProcessTestsBase.NextId);
                    if (i > 0)
                        activityData.Task = task;
                    else // Append another completed activity where the task is missing later
                        activityData.Task = new TaskTransition<UnmountActivity>(new MountingParameters(), null) { Id = 1337 };
                    activityData.Resource = new CellReference(i * 100);
                    activityData.State = ActivityState.Running;

                    originalDataInstances.Add(activityData.Id, activityData);
                    dataInstancesForLoadProperties.Add(activityData.Id, new ActivityData(new MountActivity { Id = activityData.Id }));
                }

                // Set half of the created activities to completed and give them a result.
                for (int i = 0; i < 3; i++)
                {
                    var activityData = originalDataInstances.Values.ElementAt(i);
                    activityData.State = ActivityState.Completed;
                    activityData.Result = new ActivityResult { Success = true, Numeric = 1 };
                }
            }

            _storage.SaveProcess(processData);

            using (var uow = _unitOfWorkFactory.Create())
            {
                var taskMap = new Dictionary<long, ITask> { { 42, task } };
                var completedActivities = _storage.LoadCompletedActivities(uow, CreateProcessData(jobId, processId), taskMap);
                Assert.That(completedActivities.Count, Is.EqualTo(activityCount / 2)); // 1 completed is skipped

                // Load the completed activities
                foreach (var activityPair in dataInstancesForLoadProperties)
                {
                    var sourceActivityData = originalDataInstances[activityPair.Key];
                    var loadedActivityData = activityPair.Value;

                    if (sourceActivityData.State == ActivityState.Completed && sourceActivityData.Task.Id == 42)
                    {
                        loadedActivityData = completedActivities.First(a => a.Id == activityPair.Key);
                    }
                    else
                    {
                        var repo = uow.GetRepository<IActivityEntityRepository>();
                        var entity = repo.GetByKey(loadedActivityData.Id);
                        loadedActivityData.Resource = new CellReference(entity.ResourceId);
                    }

                    Assert.That(loadedActivityData.Id, Is.EqualTo(sourceActivityData.Id), "Id mismatch in LoadActivityProperties");
                    Assert.That(loadedActivityData.Resource.Id, Is.EqualTo(sourceActivityData.Resource.Id), "ResourceId mismatch in LoadActivityProperties");
                }
            }
        }

        [Test]
        public void TestAddProcess()
        {
            ProcessData processData;
            using (var uow = _unitOfWorkFactory.Create())
            {
                var jobEntity = CreateJobEntity(uow);
                uow.SaveChanges();
                //Arrange
                processData = CreateProcessData(jobEntity.Id, IdShiftGenerator.Generate(jobEntity.Id, ProcessTestsBase.NextId));
                processData.EntityCreated = false;
                processData.State = ProcessState.Running;
            }

            //Act
            _storage.SaveProcess(processData);

            using (var uow = _unitOfWorkFactory.Create())
            {
                var dbEntity = uow.GetRepository<IProcessEntityRepository>().GetByKey(processData.Process.Id);

                //Assert
                Assert.That(dbEntity, Is.Not.Null, "AddProcess did not store the process to the database.");
                Assert.That(dbEntity.State, Is.EqualTo((int)processData.State), "AddProcess did not assign the correct state.");
                Assert.That(dbEntity.JobId, Is.EqualTo(processData.Job.Id), "AddProcess did not assign the correct JobId.");
            }
        }

        [Test]
        public void TestUpdateProcess()
        {
            ProcessData processData;
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var jobEntity = CreateJobEntity(uow);
                uow.SaveChanges();
                processData = CreateProcessData(jobEntity.Id, 0);
                processData.EntityCreated = false;
                processData.Id = IdShiftGenerator.Generate(jobEntity.Id, ProcessTestsBase.NextId);
                processData.State = ProcessState.Running;

                _storage.SaveProcess(processData);

                uow.SaveChanges();

                processData.State = ProcessState.Success;

                // Act
                _storage.SaveProcess(processData);
                uow.SaveChanges();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                // Assert
                var dbEntity = uow.GetRepository<IProcessEntityRepository>().GetByKey(processData.Process.Id);
                Assert.That(dbEntity.State, Is.EqualTo((int)processData.State), "UpdateProcess did not update the ProcessState.");
                Assert.That(dbEntity.JobId, Is.EqualTo(processData.Job.Id), "UpdateProcess did not assign the correct JobId.");
            }
        }

        [Test]
        public void TestAddActivityProcessEntityActivityData()
        {
            ActivityData activityData;
            long processId;
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var processEntity = uow.GetRepository<IProcessEntityRepository>().GetAllByState((int)ProcessState.Running).First();
                processId = processEntity.Id;
                var processData = CreateProcessData(processEntity.Job.Id, processEntity.Id);
                activityData = CreateActivityData(new MountActivity(), processData);
                activityData.Id = IdShiftGenerator.Generate(processId, ProcessTestsBase.NextId);
            }

            //Act
            _storage.SaveProcess(activityData.ProcessData);

            using (var uow = _unitOfWorkFactory.Create())
            {
                var dbEntity = uow.GetRepository<IActivityEntityRepository>().GetByKey(activityData.Activity.Id);

                //Assert
                Assert.That(dbEntity, Is.Not.Null, "AddActivity did not store the activity to the database.");
                Assert.That(processId, Is.EqualTo(dbEntity.ProcessId), "AddActivity did not store the processId to the database.");
                Assert.That(null, Is.EqualTo(dbEntity.Result), "AddActivity did not store the state to the database.");
            }
        }

        public enum TracingType
        {
            NullTracing,
            ProcessHolderTracing,
            CustomTracing
        }

        [DataContract]
        public class CustomTracing : ProcessHolderTracing
        {
            [DataMember]
            public int Foo { get; set; }
        }
        [TestCase(TracingType.NullTracing, Description = "UpdateList activity using default tracing")]
        [TestCase(TracingType.ProcessHolderTracing, Description = "UpdateList activity using process holder tracing")]
        [TestCase(TracingType.CustomTracing, Description = "UpdateList activity using custom tracing")]
        public void TestUpdateActivity(TracingType tracingType)
        {
            ActivityData activityData;
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var processEntity = uow.GetRepository<IProcessEntityRepository>().GetAllByState((int)ProcessState.Running).First();
                var processData = CreateProcessData(processEntity.Job.Id, processEntity.Id);
                activityData = CreateActivityData(new MountActivity(), processData);
                activityData.Resource = new CellReference(10);
                ((Tracing)activityData.Tracing).ResourceId = activityData.Resource.Id;

                var activityEntityId = activityData.Id = IdShiftGenerator.Generate(processEntity.Id, ProcessTestsBase.NextId);
                uow.DbContext.ActivityEntities.Add(new()
                {
                    Id = activityEntityId,
                    TaskId = activityData.Task.Id,
                    ResourceId = activityData.Resource.Id,
                    ProcessId = processData.Id,
                });

                uow.SaveChanges();
                activityData.EntityCreated = true;
            }
            activityData.Resource = new CellReference(25);

            var activity = activityData.Activity;
            activity.Result = new ActivityResult
            {
                Numeric = 42,
                Success = true
            };
            activityData.State = ActivityState.Completed;

            switch (tracingType)
            {
                case TracingType.NullTracing:
                    activity.Tracing.Started = DateTime.Now.AddMinutes(-2);
                    activity.Tracing.Completed = DateTime.Now;
                    break;
                case TracingType.ProcessHolderTracing:
                    activity.TransformTracing<ProcessHolderTracing>()
                        .Trace(trace => trace.HolderId = 42);
                    break;
                case TracingType.CustomTracing:
                    activity.TransformTracing<CustomTracing>()
                        .Trace(trace => trace.HolderId = 42)
                        .Trace(trace => trace.Foo = 42);
                    break;
            }

            _storage.SaveProcess(activityData.ProcessData);

            using (var uow = _unitOfWorkFactory.Create())
            {
                // Assert UpdateList to DBActivity
                var dbActivity = uow.GetRepository<IActivityEntityRepository>().GetByKey(activityData.Activity.Id);

                Assert.That(activityData.Resource.Id, Is.EqualTo(dbActivity.ResourceId), "UpdateActivity did not update DB-Object.");

                //Assert ActivityResult was created in the Database
                Assert.That(dbActivity, Is.Not.Null, "TheActivityResult was not created");
                Assert.That(activityData.Activity.Result.Numeric, Is.EqualTo(dbActivity.Result), "The results are not equal");
                Assert.That(activityData.Activity.Result.Success, Is.EqualTo(dbActivity.Success), "The result.success properties are not equal");

                // Assert tracing was created and restored
                var restored = new ActivityData(activityData.Activity);
                var task = new TaskTransition<MountActivity>(new MountingParameters(), null) { Id = 42 };
                _storage.LoadCompletedActivities(uow, activityData.ProcessData,
                    new Dictionary<long, ITask>
                    {
                        {42, task}
                    });
                switch (tracingType)
                {
                    case TracingType.NullTracing:
                        Assert.That(dbActivity.TracingData, Is.Null);
                        Assert.That(dbActivity.Started, Is.Not.Null);
                        Assert.That(dbActivity, Is.Not.Null);
                        Assert.That(restored.Activity.Tracing, Is.InstanceOf<Tracing>());
                        break;
                    case TracingType.ProcessHolderTracing:
                        Assert.That(dbActivity.TracingData, Is.Null);
                        Assert.That(dbActivity.ProcessHolderId, Is.EqualTo(42));
                        Assert.That(restored.Activity.Tracing, Is.InstanceOf<ProcessHolderTracing>());
                        break;
                    case TracingType.CustomTracing:
                        Assert.That(dbActivity.TracingData, Is.Not.Null);
                        Assert.That(dbActivity.TracingData, Is.EqualTo("{\"Foo\":42}"));
                        Assert.That(restored.Activity.Tracing, Is.InstanceOf<CustomTracing>());
                        var casted = (CustomTracing)restored.Activity.Tracing;
                        Assert.That(casted.Foo, Is.EqualTo(42));
                        break;
                }
            }
        }
        #endregion

        #region helper
        private ActivityData CreateActivityData(Activity activity, ProcessData processData)
        {
            var activityData = new ActivityData(activity)
            {
                Resource = new CellReference(42),
                Task = new TaskTransition<MountActivity>(null, null) { Id = 42 },
                ProcessData = processData,
                State = ActivityState.Running,
            };
            processData.AddActivity(activityData);
            return activityData;
        }

        private ProcessData CreateProcessData(long jobId, long processId)
        {
            var recipe = DummyRecipe.BuildRecipe();

            var job = new ProductionJobData(new DummyRecipe(), 0);
            ((IPersistentObject)job).Id = jobId;

            var process = new ProcessData(new Process { Recipe = recipe, Id = processId })
            {
                Job = job,
                EntityCreated = true
            };

            return process;
        }

        private static ProcessEntity CreateProcessEntity(IUnitOfWork<ProcessContext> uow, string typeName, int state, long jobId)
        {
            var process = uow.DbContext.ProcessEntities.Add(new()
            {
                Id = IdShiftGenerator.Generate(jobId << 14, ProcessTestsBase.NextId),
                TypeName = typeName,
                State = state,
                JobId = jobId,
            });

            return process.Entity;
        }

        private static JobEntity CreateJobEntity(IUnitOfWork uow)
        {
            var jobRepo = uow.GetRepository<IJobEntityRepository>();
            var entity = jobRepo.Create();
            entity.RecipeId = _index;
            entity.RecipeProvider = "RecipeProviderMock";
            entity.Amount = _index;
            entity.State = JobStateBase.InitialKey;
            _index++;
            return entity;
        }

        private void CreateDatabaseItems()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                //Create two jobs and attach them to different numbers of processes
                _jobEntities[CreateJobEntity(uow)] = 3;
                _jobEntities[CreateJobEntity(uow)] = 5;

                uow.SaveChanges();

                foreach (var jobEntity in _jobEntities.Keys)
                {
                    int count = _jobEntities[jobEntity];

                    for (int i = 0; i <= count; i++)
                    {
                        var state = i == 0 ? (int)ProcessState.Success : (int)ProcessState.Running;
                        CreateProcessEntity(uow, nameof(ProductionProcess), state, jobEntity.Id);
                    }
                }

                uow.SaveChanges();
            }
        }
        #endregion
    }
}

