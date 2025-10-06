// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Setups;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.ControlSystem.ProcessEngine.Jobs.Components;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Job storage helper class to execute job saving operations
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IJobStorage), typeof(IJobHistory))]
    internal class JobStorage : IJobStorage, IJobHistory, ILoggingComponent
    {
        #region Dependencies

        /// <summary>
        /// Factory to create job objects
        /// </summary>
        public IJobDataFactory JobFactory { get; set; }

        /// <summary>
        /// Known recipe providers required to restore jobs
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Setup provider
        /// </summary>
        public ISetupProvider SetupProvider { get; set; }

        /// <summary>
        /// Recipe provider for temporary clean-up
        /// </summary>
        public TemporarySetupProvider CleanupProvider { get; set; }

        /// <summary>
        /// All recipe providers
        /// </summary>
        public IEnumerable<IRecipeProvider> RecipeProviders => SetupProvider == null
            ? new IRecipeProvider[] { ProductManagement, CleanupProvider } : [ProductManagement, CleanupProvider, SetupProvider];

        /// <summary>
        /// Unit of work factory to open a database context
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UnitOfWorkFactory { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Logger for the JobStorage interactions
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Lock for access to _currentContext and _contextUsers
        /// </summary>
        private readonly List<IJobData> _processedJobs = new();

        /// <summary>
        /// Counter for pending storage tasks
        /// </summary>
        private int _pendingTasks = 0;

        #region LifeCycle

        public void Start() { }

        public void Stop()
        {
            // Await all pending storage tasks
            while (_pendingTasks > 0)
                Thread.Sleep(100);
        }

        #endregion

        /// <summary>
        /// Gets all jobs from the database and will map the state and recipe providers
        /// </summary>
        public IReadOnlyList<IJobData> GetAll()
        {
            using var uow = UnitOfWorkFactory.Create();

            // Get all uncompleted jobs from database
            var jobRepo = uow.GetRepository<IJobEntityRepository>();
            var dbJobs = jobRepo.Linq.Active().Where(j => j.State != JobStateBase.CompletedKey).ToList();

            // Target collection for the sorted jobs
            var jobs = new List<IJobData>(dbJobs.Count);

            // Find the job without a previous job as the first job
            var nextJob = dbJobs.FirstOrDefault(j => j.PreviousId == null);
            while (nextJob != null)
            {
                var jobData = Get(nextJob);

                dbJobs.Remove(nextJob);
                jobs.Add(jobData);

                nextJob = dbJobs.FirstOrDefault(j => j.PreviousId == nextJob.Id);
            }

            // If there are still jobs left, the database was corrupted
            if (dbJobs.Any())
                throw new InvalidOperationException("Failed to restore sorted jobs. Database corrupted!");

            // return sorted list
            return jobs;
        }

        /// <summary>
        /// Get a job from the database
        /// </summary>
        private IJobData Get(JobEntity entity)
        {
            // Resolve provider
            var foundProvider = RecipeProviders.FirstOrDefault(r => r.Name.Equals(entity.RecipeProvider));
            if (foundProvider is null)
            {
                Logger.LogError("Could not find recipe provider {provider} to load recipe {recipe} for job {job}.", foundProvider, entity.RecipeId, entity.RecipeId);
                return new BrokenJobData() { Id = entity.Id, Amount = entity.Amount };
            }

            try
            {
                // Resolve recipe
                var recipe = (IWorkplanRecipe)foundProvider.LoadRecipe(entity.RecipeId);
                // Create job data from entity
                return JobFactory.Restore(entity, recipe);
            }
            catch (RecipeNotFoundException e) // ToDo other exceptions
            {
                Logger.LogError(e, "Could not load recipe {recipe} for job {job} from {provider}", entity.RecipeId, entity.RecipeId, foundProvider);
                return new BrokenJobData() { Id = entity.Id, Amount = entity.Amount, RecipeProvider = foundProvider };
            }
        }

        /// <inheritdoc />
        Job IJobHistory.Get(long jobId)
        {
            using var uow = UnitOfWorkFactory.Create();
            var repo = uow.GetRepository<IJobEntityRepository>();
            var result = (from entity in repo.Linq
                          where entity.Id == jobId
                          select new QueryJob
                          {
                              Id = entity.Id,
                              EntityAmount = entity.Amount,
                              Classification = JobClassification.Completed,
                              SuccessCount = entity.Processes.Count(p => p.State == (int)ProcessState.Success),
                              FailureCount = entity.Processes.Count(p => p.State == (int)ProcessState.Failure),
                              ReworkedCount = entity.Processes.Count(p => p.JobId == jobId && p.Rework &&
                                                                          (p.State == (int)ProcessState.Success ||
                                                                           p.State == (int)ProcessState.Failure)),
                              RecipeId = entity.RecipeId,

                              StateKey = entity.State,
                              RecipeProvider = entity.RecipeProvider
                          }).FirstOrDefault();

            if (result == null)
                return null;

            if (result.StateKey != JobStateBase.CompletedKey)
                throw new InvalidOperationException("Only completed jobs can be loaded by the history.");

            var provider = RecipeProviders.First(p => p.Name == result.RecipeProvider);
            if (provider is not IProductManagement)
                throw new InvalidOperationException("Only jobs based on product recipes can be reloaded");

            result.DelayedRecipe = provider.LoadRecipe(result.RecipeId);
            return result;
        }

        /// <summary>
        /// Update the storage from the current job list
        /// This also hides completed jobs AND updates the entity links
        /// </summary>
        public void Save(ModifiedJobsFragment modifiedJobs)
        {
            ConcurrentAccess(modifiedJobs.AffectedJobs, ExecuteSave, modifiedJobs);
        }

        private static void ExecuteSave(IUnitOfWork uow, ModifiedJobsFragment modifiedJobs)
        {
            var jobRepo = uow.GetRepository<IJobEntityRepository>();

            // Extract argument values
            var jobs = modifiedJobs.AffectedJobs;
            var previousId = modifiedJobs.PreviousId;

            // Fetch all affected entities, including the previous id, in a single query
            var ids = jobs.Select(j => j.Id)
                .Concat(previousId.HasValue ? [previousId.Value] : Enumerable.Empty<long>())
                .ToArray();
            var jobEntities = jobRepo.GetByKeys(ids);

            // Fetch and validate previous job
            var previousJob = previousId.HasValue
                ? jobEntities.First(j => j.Id == previousId.Value) : null;

            foreach (var job in jobs)
            {
                // Fetch entity and update state OR create new one
                var jobEntity = jobEntities.FirstOrDefault(j => j.Id == job.Id);
                if (jobEntity == null)
                    jobEntity = Add(uow, job, previousJob);
                else if (jobEntity.State != job.State.Key)
                    jobEntity.State = job.State.Key;

                // Do not update previousJob for completed jobs to close the gap
                if (job.State.Classification == JobClassification.Completed)
                {
                    // Clear link to previous entity on completed jobs
                    jobEntity.Previous = null;
                    continue;
                }

                // Update previous link
                if (jobEntity.Previous != previousJob)
                    jobEntity.Previous = previousJob;

                previousJob = jobEntity;
            }
        }

        public void UpdateState(IJobData jobData, IJobState newState)
        {
            ParallelOperations.ScheduleExecution(() => ConcurrentAccess([jobData], ExecuteUpdateState, jobData), 0, 0);
        }

        private static void ExecuteUpdateState(IUnitOfWork uow, IJobData jobData)
        {
            var jobRepo = uow.GetRepository<IJobEntityRepository>();
            var jobEntity = jobRepo.GetByKey(jobData.Id);

            var stateKey = jobData.State.Key;
            // Completion should only be set by CompletionFragment
            if (jobEntity.State != stateKey && stateKey != JobStateBase.CompletedKey)
                jobEntity.State = stateKey;
        }

        private void ConcurrentAccess<TArgument>(IReadOnlyList<IJobData> modifiedJobs, Action<IUnitOfWork, TArgument> callback, TArgument argument)
        {
            Interlocked.Increment(ref _pendingTasks);
            // Wait until no other thread processes the same jobs
            while (true)
            {
                lock (_processedJobs)
                {
                    if (!_processedJobs.Any(modifiedJobs.Contains))
                    {
                        // Add our jobs to the currently modified jobs
                        _processedJobs.AddRange(modifiedJobs);
                        break;
                    }
                }

                Thread.Sleep(1);
            }

            try
            {
                // Unique access to the database for the modified jobs
                using var uow = UnitOfWorkFactory.Create();
                callback(uow, argument);

                uow.SaveChanges();
            }
            finally
            {
                lock (_processedJobs)
                {
                    // Remove jobs as they are no longer updated
                    foreach (var job in modifiedJobs)
                        _processedJobs.Remove(job);
                }

                Interlocked.Decrement(ref _pendingTasks);
            }
        }

        /// <summary>
        /// Adds a new job to the database repository
        /// </summary>
        private static JobEntity Add(IUnitOfWork uow, IJobData jobData, JobEntity previous)
        {
            // Create entity and link to business object
            var jobEntity = uow.CreateEntity<JobEntity>((IPersistentObject)jobData);

            // Basic properties
            jobEntity.Amount = jobData.Amount;
            jobEntity.RecipeProvider = jobData.RecipeProvider.Name;
            jobEntity.RecipeId = jobData.Recipe.Id;

            // Frequently changed properties
            jobEntity.State = jobData.State.Key;
            jobEntity.Previous = previous;

            return jobEntity;
        }

        private class QueryJob : Job
        {
            internal long RecipeId { get; set; }

            internal int StateKey { get; set; }

            internal string RecipeProvider { get; set; }

            internal IRecipe DelayedRecipe
            {
                get => Recipe;
                set => Recipe = value;
            }

            internal int EntityAmount
            {
                get => Amount;
                set => Amount = value;
            }

            public QueryJob() : base(null, 0)
            {
                RunningProcesses = Array.Empty<IProcess>();
                AllProcesses = Array.Empty<IProcess>();
            }
        }
    }
}
