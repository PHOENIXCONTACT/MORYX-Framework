// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.Logging;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup;

[Component(LifeCycle.Singleton, typeof(IJobHandler), typeof(ISetupJobHandler))]
internal class SetupJobHandler : ILoggingComponent, ISetupJobHandler
{
    #region Dependencies

    /// <summary>
    /// Logger for this component
    /// </summary>
    public IModuleLogger Logger { get; set; }

    /// <summary>
    /// Job list providing access to all jobs
    /// </summary>
    public IJobDataList JobList { get; set; }

    /// <summary>
    /// Castle factory to create <see cref="ISetupTrigger"/> instances from
    /// their <see cref="SetupTriggerConfig"/>
    /// </summary>
    public ISetupProvider SetupProvider { get; set; }

    /// <summary>
    /// Temporary recipe provider
    /// </summary>
    public TemporarySetupProvider RecipeProvider { get; set; }

    /// <summary>
    /// Factory to create jobs from recipes
    /// </summary>
    public IJobDataFactory JobFactory { get; set; }

    /// <summary>
    /// Resource management to check if the resource management
    /// provides the requested capabilities
    /// </summary>
    public IResourceManagement ResourceManagement { get; set; }

    #endregion

    /// <inheritdoc />
    public void Start()
    {
        JobList.StateChanged += OnJobStateChanged;
    }

    public void Stop()
    {
        JobList.StateChanged -= OnJobStateChanged;
    }

    /// <inheritdoc/>
    public void Handle(LinkedList<IJobData> newJobs)
    {
        // Start from the first job and prepare them as needed
        var current = newJobs.First;
        while (current != null)
        {
            current = HandleCurrentJob(newJobs, current);
        }
    }

    /// <summary>
    /// Creates and inserts setup and/or clean up for the <paramref name="current"/>
    /// job. Returns the next job to be handled.
    /// </summary>
    private LinkedListNode<IJobData> HandleCurrentJob(LinkedList<IJobData> newJobs, LinkedListNode<IJobData> current)
    {
        // Filter all none production jobs
        if (current.Value is not IProductionJobData productionJob)
        {
            return current.Next;
        }

        if (RequiresSetupCreation(productionJob))
        {
            SetupRecipe prepareRecipe;
            try
            {
                prepareRecipe = SetupProvider?.RequiredSetup(SetupExecution.BeforeProduction, productionJob.Recipe, new CurrentResourceTarget(ResourceManagement));
            }
            catch (Exception setupProviderException)
            {
                return InterruptJobsForward(current, setupProviderException, newJobs);
            }

            if (prepareRecipe != null)
            {
                var prepare = JobFactory.Create<ISetupJobData>(prepareRecipe, 1);
                newJobs.AddBefore(current, prepare);
            }
        }

        if (RequiresCleanUpCreation(newJobs, productionJob))
        {
            SetupRecipe cleanupRecipe;
            try
            {
                // Clean-up is evaluated just in time, so we only create a temporary recipe
                cleanupRecipe = SetupProvider?.RequiredSetup(SetupExecution.AfterProduction, productionJob.Recipe, new TemporaryCleanupTarget()) ??
                                RecipeProvider.CreateTemporary(productionJob.Recipe);
            }
            catch (Exception e)
            {
                InterruptJobsBackward(current, e, newJobs);
                return current.Next;
            }

            var cleanup = JobFactory.Create<ISetupJobData>(cleanupRecipe, 1);
            // ReSharper disable once AccessToModifiedClosure
            newJobs.AddAfter(current, cleanup);
        }

        return current.Next;
    }

    /// <summary>
    /// Determine whether a setup job needs to be created for the <paramref name="currentNode"/>
    /// </summary>
    private bool RequiresSetupCreation(IJobData currentNode)
    {
        var previous = JobList.Previous(currentNode);
        var followingCleanUp = FollowingCleanup(currentNode);
        return
            // If the direct previous job is of the same recipe, we can reuse its pre-execution setup as long as it is still running
            (previous?.Recipe.Id != currentNode.Recipe.Id || previous.Classification > JobClassification.Running)
            // If there is no previous job, but the clean-up was not started, the setup is still valid
            && (followingCleanUp?.Classification is null || followingCleanUp.Classification != JobClassification.Waiting);
    }

    /// <summary>
    /// Iterate forward on job list to find a clean-up, skipping only production
    /// jobs of the same recipe
    /// </summary>
    private ISetupJobData FollowingCleanup(IJobData jobData)
    {
        var recipeId = jobData.Recipe.Id;
        foreach (var next in JobList.Forward(jobData))
        {
            // Skip all identical production jobs
            if (next.Recipe.Id == recipeId)
                continue;

            if (next is ISetupJobData cleanup && cleanup.Recipe.TargetRecipe.Id == recipeId)
                return cleanup;
            break;
        }
        return null;
    }

    /// <summary>
    /// Abort the <paramref name="current"/> and all following jobs that would cause the
    /// same exception during cleanup creation as they share the same recipe
    /// </summary>
    /// <param name="current">The job that caused the exception initially</param>
    /// <param name="e">The exception to be logged for all jobs</param>
    /// <param name="newJobs">The list of new jobs to remove from</param>
    /// <returns>The next job to be considered save for processing</returns>
    private LinkedListNode<IJobData> InterruptJobsForward(LinkedListNode<IJobData> current, Exception e, LinkedList<IJobData> newJobs)
    {
        var abortedJobIds = new List<long>() { };
        var jobToBeAborted = current;
        while (true)
        {
            jobToBeAborted.Value.Interrupt();
            abortedJobIds.Add(jobToBeAborted.Value.Id);
            var next = jobToBeAborted.Next;
            newJobs.Remove(jobToBeAborted); // Should I remove the jobs from the linked list here? They would cause an exception when ready is called
            if (LastOfRecipe(newJobs, jobToBeAborted.Value))
            {
                LogAbortedJobs(e, abortedJobIds, SetupExecution.BeforeProduction);
                return next;
            }
            jobToBeAborted = next;
        }
    }

    /// <summary>
    /// Check if the current node is the last one with this recipe
    /// </summary>
    private bool LastOfRecipe(LinkedList<IJobData> newJobs, IJobData currentNode)
    {
        var next = JobList.Forward(currentNode).FirstOrDefault();
        return next == null // Its the very last job
               || next.Recipe.Id != currentNode.Recipe.Id // The next job has a different recipe
               || (next.Classification == JobClassification.Idle && !newJobs.Contains(next)); // The next job has the same recipe but isn't scheduled together with this one
    }

    private void LogAbortedJobs(Exception e, List<long> abortedJobIds, SetupExecution executionType)
    {
        Logger.LogError(e, "{provider} threw an exception when creating required setup {classification} for " +
                           "job(s) {jobs}. Interrupting job(s)...", SetupProvider.GetType().Name, executionType, string.Join(", ", abortedJobIds));
    }

    /// <summary>
    /// Determines whether a clean up job needs to be created for the <paramref name="currentNode"/>
    /// </summary>
    private bool RequiresCleanUpCreation(LinkedList<IJobData> newJobs, IJobData currentNode)
    {
        return
            // Check for clean-up if this recipe is not used by any following job
            LastOfRecipe(newJobs, currentNode)
            // See if we already have an un-started clean-up for this recipe
            && FollowingCleanup(currentNode) is null;
    }

    /// <summary>
    /// Abort the <paramref name="current"/> and all previous jobs that would cause the
    /// same exception during cleanup creation as they share the same recipe
    /// </summary>
    /// <param name="current">The job that caused the exception initially</param>
    /// <param name="e">The exception to be logged for all jobs</param>
    private void InterruptJobsBackward(LinkedListNode<IJobData> current, Exception e, LinkedList<IJobData> newJobs)
    {
        var abortedJobIds = new List<long>() { };
        var jobToBeAborted = current;
        while (true)
        {
            jobToBeAborted.Value.Interrupt();
            abortedJobIds.Add(jobToBeAborted.Value.Id);
            var previous = jobToBeAborted.Previous;
            newJobs.Remove(jobToBeAborted);
            if (FirstOfRecipe(jobToBeAborted.Value))
            {
                if (previous?.Value is SetupJobData setupJobData && setupJobData.Recipe.TargetRecipe.Id != jobToBeAborted.Value.Recipe.Id)
                    break;

                // Interrupt the setup for this set of jobs
                abortedJobIds.Add(previous.Value.Id);
                previous.Value.Interrupt();
                newJobs.Remove(previous);
                break;
            }
            jobToBeAborted = previous;
        }
        LogAbortedJobs(e, abortedJobIds, SetupExecution.AfterProduction);
    }

    /// <summary>
    /// Check if the current node is the last one with this recipe
    /// </summary>
    private bool FirstOfRecipe(IJobData currentNode)
    {
        var previous = JobList.Backward(currentNode).FirstOrDefault();
        return previous == null || previous.Recipe.Id != currentNode.Recipe.Id || previous.Classification == JobClassification.Idle;
    }

    /// <summary>
    /// Listener on the <see cref="IJobDataList.StateChanged"/> event to confirm
    /// or retry a completed setup
    /// </summary>
    private void OnJobStateChanged(object sender, JobStateEventArgs args)
    {
        var jobData = args.JobData;
        if (args.CurrentState.Classification < JobClassification.Completed)
            CheckRecipeUpdate(jobData);
        else
            OnJobCompleted(jobData);
    }

    private void CheckRecipeUpdate(IJobData jobData)
    {
        // Check if setup job requires a new recipe
        var setupJob = jobData as ISetupJobData;
        if (setupJob?.RecipeRequired != true)
            return;

        // Fetch current recipe and check if setup is complete
        var currentRecipe = setupJob.Recipe;
        var retryRecipe = SetupProvider?.RequiredSetup(currentRecipe.Execution, (ProductionRecipe)currentRecipe.TargetRecipe, new CurrentResourceTarget(ResourceManagement));
        setupJob.UpdateSetup(retryRecipe);
    }

    /// <summary>
    /// Called when a job was completed
    /// </summary>
    private void OnJobCompleted(IJobData jobData)
    {
        // Otherwise check if a production job was completed
        var productionJob = jobData as IProductionJobData;
        if (productionJob == null)
            return;

        // If we have a follow-up job with the same recipe, we don't have to do anything
        var followUp = JobList.Next(jobData)?.Recipe.Id == jobData.Recipe.Id;
        if (followUp)
            return;

        // Check if the previous job is a setup targeting the completed job
        // Clean-up jobs don't need to be aborted. They are updated when started by the scheduler
        if (JobList.Previous(jobData) is ISetupJobData previous
            && previous.Recipe is SetupRecipe setupRecipe
            && setupRecipe.Execution == SetupExecution.BeforeProduction
            && previous.Classification < JobClassification.Completing
            && previous.Recipe.TargetRecipe.Id == jobData.Recipe.Id)
        {
            previous.Abort();
        }
    }

    private class TemporaryCleanupTarget : ISetupTarget
    {
        public IReadOnlyList<ICell> Cells(ICapabilities capabilities)
        {
            // Return a match for every cell to get all possible clean-ups
            return [new CellReference()];
        }
    }

    private class CellReference : Cell, ICell
    {
        protected override IEnumerable<Session> ProcessEngineAttached()
        {
            yield break;
        }

        protected override IEnumerable<Session> ProcessEngineDetached()
        {
            yield break;
        }

        public override void StartActivity(ActivityStart activityStart)
        {
        }

        public override void SequenceCompleted(SequenceCompleted completed)
        {
        }
    }
}