// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.Processes;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Serialization;
using Moryx.Tools;
using Newtonsoft.Json;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Storage to save activities and processes to the database
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IProcessStorage))]
    internal class ProcessStorage : IProcessStorage, ILoggingComponent
    {
        private TracingWrapper[] _tracingWrappers;

        /// <summary>
        /// Product management to restore product instance
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Injected logger
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Injected factory MUST only be used for the tracing type cache
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UnitOfWorkFactory { get; set; }

        /// <inheritdoc />
        public void Start()
        {
            _tracingWrappers = ReflectionTool.GetPublicClasses<Tracing>().Select(t => new TracingWrapper(t)).ToArray();

            // Try to map each of the types to a database entity
            using var uow = UnitOfWorkFactory.Create();
            var typeRepo = uow.GetRepository<ITracingTypeRepository>();

            // Try to map each type constructor to an entity
            var typeEntities = typeRepo.GetAll();
            foreach (var tracingWrapper in _tracingWrappers)
            {
                var tracingType = tracingWrapper.Type;
                var isEntityExisting = typeEntities.Any(entity =>
                {
                    // Check if entity and type represent the same tracing
                    if (entity.NameSpace != tracingType.Namespace || entity.Classname != tracingType.Name)
                        return false;

                    tracingWrapper.Id = entity.Id;
                    return true;
                });

                if (isEntityExisting)
                    continue;

                var typeEntity = typeRepo.Create(tracingType.Assembly.GetName().Name, tracingType.Namespace, tracingType.Name);
                uow.LinkEntityToBusinessObject(tracingWrapper, typeEntity);
            }

            // Save newly created tracing types
            uow.SaveChanges();
        }

        /// <inheritdoc />
        public void Stop()
        {
            // Method is not called
        }

        /// <inheritdoc />
        public IReadOnlyList<ProcessData> GetRunningProcesses(IUnitOfWork uow, IJobData job)
        {
            var procRepo = uow.GetRepository<IProcessEntityRepository>();

            var dbProcesses = procRepo.Linq.Where(p => p.State <= (int)ProcessState.Interrupted && p.JobId == job.Id)
                .Select(process => new
                {
                    process.Id,
                    process.Rework,
                    process.ReferenceId,
                    MaxIndex = process.Activities.Select(a => a.Id).DefaultIfEmpty().Max()
                }).ToList();

            return (from dbProcess in dbProcesses
                    let process = job.Recipe.CreateProcess()
                    select new ProcessData(process)
                    {
                        Id = dbProcess.Id,
                        Recipe = job.Recipe,
                        EntityCreated = true,
                        Rework = dbProcess.Rework,
                        ReferenceId = dbProcess.ReferenceId,
                        ActivityIndex = (int)IdShiftGenerator.ExtractChild(dbProcess.MaxIndex) + 1,
                    }).ToList();
        }

        /// <inheritdoc />
        public void LoadCompletedProcesses(IUnitOfWork uow, IJobData jobData, ICollection<ProcessData> allProcesses)
        {
            var recipe = jobData.Recipe;

            var processRepo = uow.GetRepository<IProcessEntityRepository>();
            var query = processRepo.Linq.Where(p => p.State >= (int)ProcessState.Success && p.JobId == jobData.Id).ToList();

            // Prepare collection that can hold all processes
            foreach (var processEntity in query)
            {
                // Create process
                var process = recipe.CreateProcess();
                process.Id = processEntity.Id;
                // Create process data
                var processData = new ProcessData(process)
                {
                    Id = processEntity.Id,
                    Rework = processEntity.Rework,
                    State = (ProcessState)processEntity.State,

                    Job = jobData,
                    Recipe = recipe,

                    ReferenceId = processEntity.ReferenceId
                };

                // Restore product in production processes
                if (process is ProductionProcess prodProcess)
                {
                    if (processData.ReferenceId > 0) // Only load article if the process already has one!
                        prodProcess.ProductInstance = ProductManagement.GetInstance(processData.ReferenceId);
                    else // Otherwise provide a prepared, unsaved instance
                        prodProcess.ProductInstance = ((IProductRecipe)prodProcess.Recipe).Target.CreateInstance();
                }

                // Restore activities from workflow
                var context = new ProcessWorkplanContext(process);
                var taskMap = recipe.Workplan.Steps
                    .Select(step => step.CreateInstance(context))
                    .OfType<ITask>().ToDictionary(task => task.Id, task => task);
                foreach (var activityData in LoadCompletedActivities(uow, processData, taskMap))
                    processData.AddActivity(activityData);

                allProcesses.Add(processData);
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<ActivityData> LoadCompletedActivities(IUnitOfWork uow, ProcessData processData, IDictionary<long, ITask> taskMap)
        {
            var repo = uow.GetRepository<IActivityEntityRepository>();
            var dbActivities = repo.GetCompleted(processData.Id).ToList();

            var completedActivities = new List<ActivityData>(dbActivities.Count);
            foreach (var dbActivity in dbActivities.OrderBy(dbActivity => dbActivity.Started))
            {
                if (!taskMap.ContainsKey(dbActivity.TaskId))
                {
                    Logger.Log(LogLevel.Error, "Failed to restore activity {0} with task id {1}", dbActivity.Id, dbActivity.TaskId);
                    continue;
                }

                var task = taskMap[dbActivity.TaskId];

                var activity = (Activity)task.CreateActivity(processData.Process);
                FillActivity(dbActivity, activity);

                // Restore all properties
                var activityData = new ActivityData(activity)
                {
                    Task = task,
                    EntityCreated = true,
                    Resource = new CellReference(dbActivity.ResourceId),
                    State = ActivityState.Completed
                };

                completedActivities.Add(activityData);
            }

            return completedActivities;
        }

        /// <inheritdoc />
        public void TryReloadRunningActivity(long processId, ActivityData activityData)
        {
            using var uow = UnitOfWorkFactory.Create();
            var activityRepo = uow.GetRepository<IActivityEntityRepository>();
            var configured = activityRepo.GetRunning(processId);
            var match = configured.FirstOrDefault(ae => ae.TaskId == activityData.Task.Id);
            if (match == null)
                return;

            activityData.Id = match.Id;
            activityData.EntityCreated = true;
            // Reload tracing if it was previously saved
            if (match.TracingTypeId != null)
            {
                // Restore tracing from reloaded activity
                activityData.Activity.Tracing = LoadTracing(match);
            }
        }

        /// <inheritdoc />
        public void FillActivities(IUnitOfWork uow, IProcess process, IDictionary<long, ITask> taskMap)
        {
            var repo = uow.GetRepository<IActivityEntityRepository>();
            var dbActivities = repo.GetCompleted(process.Id).ToList();

            foreach (var dbActivity in dbActivities.OrderBy(dbActivity => dbActivity.Started))
            {
                if (!taskMap.ContainsKey(dbActivity.TaskId))
                {
                    Logger.Log(LogLevel.Error, "Failed to restore activity {0} with task id {1}", dbActivity.Id, dbActivity.TaskId);
                    continue;
                }

                var task = taskMap[dbActivity.TaskId];

                var activity = (Activity)task.CreateActivity(process);
                FillActivity(dbActivity, activity);
            }
        }

        /// <summary>
        /// Copy all information from the entity to the activity object
        /// </summary>
        private void FillActivity(ActivityEntity source, Activity target)
        {
            target.Id = source.Id;
            target.Tracing = LoadTracing(source);
            target.Result = new ActivityResult
            {
                Numeric = source.Result.Value,
                Success = source.Success
            };
        }

        /// <inheritdoc />
        public Tracing LoadTracing(ActivityEntity activityEntity)
        {
            // Fetch type from cache and create instance
            var tracingType = _tracingWrappers.First(tt => tt.Id == activityEntity.TracingTypeId);
            var tracing = tracingType.Constructor();

            // Base properties
            tracing.Started = activityEntity.Started;
            tracing.Completed = activityEntity.Completed;
            tracing.Text = activityEntity.TracingText;
            tracing.Progress = activityEntity.Progress;
            tracing.ResourceId = activityEntity.ResourceId;

            // Process holder tracing
            if (tracing is ProcessHolderTracing processHolderTracing)
                processHolderTracing.HolderId = activityEntity.ProcessHolderId ?? -1;

            // Load extended properties from JSON if there is any
            if (activityEntity.TracingData?.Length > 2)
                JsonConvert.PopulateObject(activityEntity.TracingData, tracing);

            return tracing;
        }

        /// <inheritdoc />
        public void SaveProcess(ProcessData processData)
        {
            using var uow = UnitOfWorkFactory.Create();
            var processRepository = uow.GetRepository<IProcessEntityRepository>();

            // Get or create process
            ProcessEntity entity;
            if (processData.EntityCreated)
            {
                entity = processRepository.GetByKey(processData.Id);
            }
            else
            {
                entity = processRepository.Create();
                entity.Id = processData.Id;
                entity.TypeName = processData.Process.GetType().Name;
                entity.State = (int)processData.State;
                entity.JobId = processData.Job.Id;
            }
            entity.State = (int)processData.State;
            entity.Rework = processData.Rework;
            entity.ReferenceId = processData.ReferenceId;

            // Save unsaved activities
            // Only save activities that were delivered to a cell AND not saved yet
            var unsavedActivities = processData.Activities
                .Where(a => a.State >= ActivityState.Running).ToList();
            foreach (var activity in unsavedActivities)
            {
                SaveActivity(uow, activity);
            }

            uow.SaveChanges();

            foreach (var activityData in unsavedActivities)
            {
                activityData.EntityCreated = true;
            }
            processData.EntityCreated = true;
        }

        /// <inheritdoc />
        public void AddCompletedActivity(ProcessData processData, ActivityData activityData)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                // If the process does not exist yet, create it
                if (!processData.EntityCreated)
                {
                    uow.DbContext.Processes.Add(new ProcessEntity
                    {
                        Id = processData.Id,
                        TypeName = processData.Process.GetType().Name,
                        JobId = processData.Job.Id,
                        State = (int)processData.State,
                        Rework = processData.Rework,
                        ReferenceId = processData.ReferenceId,
                    });
                }

                // Save completed activity
                SaveActivity(uow, activityData);

                // Save
                uow.SaveChanges();

                processData.EntityCreated = true;
                activityData.EntityCreated = true;
            }
        }

        /// <summary>
        /// Save an activity in the database
        /// </summary>
        private void SaveActivity(IUnitOfWork<ProcessContext> uow, ActivityData activityData)
        {
            var activity = activityData.Activity;
            var activityRepo = uow.GetRepository<IActivityEntityRepository>();

            ActivityEntity dbActivity;
            if (activityData.EntityCreated)
            {
                dbActivity = activityRepo.GetByKey(activity.Id);
            }
            else
            {
                dbActivity = activityRepo.Create();
                dbActivity.Id = activity.Id;
                dbActivity.TaskId = activityData.Task.Id;
                dbActivity.ResourceId = activityData.Resource.Id;
                dbActivity.ProcessId = activityData.ProcessData.Id;
            }

            // UpdateList tracing info
            SaveTracing(activity, dbActivity);

            // Only set the other values for completed activities
            if (activityData.Result == null)
                return;

            // Save result
            var activityResult = activity.Result;
            dbActivity.Result = activityResult.Numeric;
            dbActivity.Success = activityResult.Success;
        }

        /// <summary>
        /// Save tracing information to activity
        /// </summary>
        /// <param name="source">Data source</param>
        /// <param name="target">Entity target</param>
        private void SaveTracing(IActivity source, ActivityEntity target)
        {
            // Fetch tracing type cache and save reference on target
            var tracingType = _tracingWrappers.First(tt => tt.TypeCheck(source.Tracing));
            target.TracingTypeId = tracingType.Id;

            // General tracing data
            target.Started = ConvertToUtc(source.Tracing.Started);
            target.Completed = ConvertToUtc(source.Tracing.Completed);
            target.TracingText = source.Tracing.Text;
            target.Progress = source.Tracing.Progress;
            target.ResourceId = source.Tracing.ResourceId;

            // Process holder tracing data
            if (source.Tracing is ProcessHolderTracing processHolderTracing)
                target.ProcessHolderId = processHolderTracing.HolderId;

            // Extended tracing data
            var json = JsonConvert.SerializeObject(source.Tracing, JsonSettings.Minimal);
            if (json.Length > 2)
                target.TracingData = json;
        }

        private static DateTime? ConvertToUtc(DateTime? dateTime)
        {
            if (dateTime == null) return null;

            var nonNullDateTime = (DateTime)dateTime;
            if (nonNullDateTime.Kind == DateTimeKind.Utc)
                return nonNullDateTime;
            if (nonNullDateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTimeToUtc(nonNullDateTime, TimeZoneInfo.Local);
            throw new ArgumentException($"Provided {nameof(DateTime)} is neither UTC nor Local Time");
        }

        private class TracingWrapper : IPersistentObject
        {
            public long Id { get; set; }

            public Type Type { get; }

            public Func<Tracing> Constructor { get; }

            public Func<object, bool> TypeCheck { get; }

            public TracingWrapper(Type type)
            {
                Type = type;
                Constructor = ReflectionTool.ConstructorDelegate<Tracing>(type);
                TypeCheck = ReflectionTool.TypePredicate(type, false);
            }
        }
    }
}
