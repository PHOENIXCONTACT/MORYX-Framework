// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.Logging;
using Moryx.Model.Repositories;
using Moryx.Notifications;
using Moryx.Serialization;
using Moryx.Workplans;
using Newtonsoft.Json;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// The activity provider holds the <see cref="IWorkplanEngine"/> instances and
    /// is responsible for adding activities to the pool, when a process was added
    /// or a previous activity completed
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener))]
    internal sealed class ActivityProvider : IActivityPoolListener, ILoggingComponent, IDisposable, INotificationSender
    {
        /// <summary>
        /// Identifier for notifications send by the ResourceAssignment
        /// </summary>
        private const string NotificationSenderName = nameof(ActivityProvider);

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild(nameof(ActivityProvider))]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Process storage to load activities
        /// </summary>
        public IProcessStorage ProcessStorage { get; set; }

        /// <summary>
        /// The one and only activity pool
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Used to handle notifications
        /// </summary>
        public INotificationAdapter NotificationAdapter { get; set; }

        /// <summary>
        /// Injected factory to open a database connection
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UnitOfWorkFactory { get; set; }

        private CountdownEvent _shutdownSync;

        /// <inheritdoc />
        public int StartOrder => 40;

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
            ActivityPool.ActivityChanged += OnActivityChanged;
        }

        public void Start()
        {
            // Create the countdown event with 1 which represents the call to Stop()
            _shutdownSync = new CountdownEvent(1);
        }

        public void Stop()
        {
            // Signal the shutdown from the ProcessEngine
            _shutdownSync?.Signal();

            // Wait for the countdown to reach 0, if it hasn't already
            _shutdownSync?.Wait();
        }

        public void Dispose()
        {
            ActivityPool.ProcessChanged -= OnProcessChanged;
            ActivityPool.ActivityChanged -= OnActivityChanged;
        }

        #region Pool events

        /// <summary>
        /// Start execution when the process is ready
        /// </summary>
        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            var processData = args.ProcessData;
            switch (args.Trigger)
            {
                case ProcessState.Initial:
                    PrepareEngine(processData);
                    break;
                case ProcessState.Ready:
                    StartWorkflow(processData);
                    break;
                case ProcessState.RestoredReady:
                    LoadWorkflow(processData);
                    break;
                case ProcessState.CleaningUpReady:
                    LoadWorkflow(processData, true);
                    break;
                case ProcessState.Interrupted:
                    SuspendWorkflow(processData);
                    break;
                case ProcessState.Success:
                case ProcessState.Failure:
                    RemoveWorkflow(processData);
                    NotificationAdapter.AcknowledgeAll(this, processData);
                    break;
            }
        }

        /// <summary>
        /// Directly start a workflow
        /// </summary>
        private void StartWorkflow(ProcessData processData)
        {
            PrepareEngine(processData);
            processData.Engine.Start();
            ActivityPool.UpdateProcess(processData, ProcessState.EngineStarted);
        }

        /// <summary>
        /// Load workflow for a restored process from the database, and handle all processes as broken if the cleanup-flag is true
        /// </summary>
        private void LoadWorkflow(ProcessData processData, bool cleanup = false)
        {
            using var uow = UnitOfWorkFactory.Create();
            // Load snapshot from database
            var snapshot = LoadSnapshot(uow, processData);

            // Restore workflow from snapshot
            var workflowEngine = processData.Engine;
            workflowEngine.Restore(snapshot);

            // Restore completed activities
            var taskMap = workflowEngine.ExecutedWorkplan.Transitions.OfType<ITask>().ToDictionary(t => t.Id, t => t);
            var activities = ProcessStorage.LoadCompletedActivities(uow, processData, taskMap);

            // Restore completed activities on pool
            foreach (var activity in activities)
            {
                processData.AddActivity(activity);
                activity.ProcessData = processData;
            }

            // Check if the workflow can be resumed
            // We do this after loading it to make sure we can follow the normal removal trigger
            if (snapshot.Holders.Length == 0 || cleanup)
            {
                Logger.Log(LogLevel.Error, "Can not resume process {0} {1}", processData.Id, cleanup ? "because of job-cleanup." : "without a snapshot.");
                ActivityPool.UpdateProcess(processData, ProcessState.RemoveBroken);
            }
            else
            {
                workflowEngine.Start();
                ActivityPool.UpdateProcess(processData, ProcessState.EngineStarted);
            }
        }

        /// <summary>
        /// Load workflow snapshot for a process and delete the holder entities
        /// </summary>
        private static WorkplanSnapshot LoadSnapshot(IUnitOfWork uow, ProcessData processData)
        {
            // Load holders from db
            var repo = uow.GetRepository<ITokenHolderEntityRepository>();
            var holders = repo.GetAllByProcessId(processData.Id);

            // Create snapshot from holders
            var snapshot = new WorkplanSnapshot
            {
                Holders = (from holder in holders
                           select new HolderSnapshot
                           {
                               HolderId = holder.HolderId,
                               Tokens = JsonConvert.DeserializeObject<IToken[]>(holder.Tokens, JsonSettings.Minimal)
                           }).ToArray()
            };

            // Remove holder entities after snapshot was created
            repo.RemoveRange(holders);
            uow.SaveChanges();
            return snapshot;
        }

        /// <summary>
        /// Workflow was interrupted and its current state shall be saved in the database
        /// </summary>
        private void SuspendWorkflow(ProcessData processData)
        {
            _shutdownSync.AddCount();

            try
            {
                using var uow = UnitOfWorkFactory.Create();
                // Workflow was not loaded and its snapshot is still valid
                if (processData.ActivityCount() == 0)
                    return;

                // Create snapshot and write to db
                var holderRepo = uow.GetRepository<ITokenHolderEntityRepository>();
                var snapshot = processData.Engine.Pause();
                foreach (var holder in snapshot.Holders)
                {
                    // Create tokens as JSON for all holders
                    var tokens = JsonConvert.SerializeObject(holder.Tokens, JsonSettings.Minimal);

                    // Create holder
                    var holderEntity = holderRepo.Create(holder.HolderId, tokens);
                    holderEntity.ProcessId = processData.Id;
                }

                uow.SaveChanges();

                RemoveWorkflow(processData);
            }
            finally
            {
                _shutdownSync.Signal();
            }
        }

        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            if (args.Trigger != ActivityState.ResultProcessed)
                return;

            // Very specific exclusion of activities that the ProcessRemoval created,
            // would be better to only include things the ActivityProvider provided
            if (args.ActivityData.Activity is ProcessFixupActivity)
                return;

            var activityData = args.ActivityData;
            activityData.Task.Completed(activityData.Result);

            ActivityPool.UpdateActivity(activityData, ActivityState.EngineProceeded);
        }

        #endregion

        #region Engine events

        private void OnWorkflowCompleted(object sender, IPlace place)
        {
            var processData = GetProcessByEngine((IWorkplanEngine)sender);
            switch (place.Classification)
            {
                case NodeClassification.End:
                    // TODO: This is not a good place, but timing sucks
                    if (processData.Process is ProductionProcess prodProcess)
                        prodProcess.ProductInstance.State = ProductInstanceState.Success;
                    ActivityPool.UpdateProcess(processData, ProcessState.Success);
                    break;
                case NodeClassification.Failed:
                    // TODO: This is not a good place, but timing sucks
                    if (processData.Process is ProductionProcess failedProcess)
                        failedProcess.ProductInstance.State = ProductInstanceState.Failure;
                    ActivityPool.UpdateProcess(processData, ProcessState.Failure);
                    break;
            }
        }

        private void OnTransitionTriggered(object sender, ITransition transition)
        {
            var processData = GetProcessByEngine((IWorkplanEngine)sender);
            // Stop creating activities for stopping processes
            if (processData.State > ProcessState.Running)
                return;

            var taskTransition = (ITaskTransition)transition;
            try
            {
                var activity = (Activity)taskTransition.CreateActivity(processData.Process);
                var activityData = new ActivityData(activity) { Task = taskTransition };
                // Try to reload the activity for restored processes, this will ONLY find a match for previously running activities
                if (processData.State == ProcessState.RestoredReady)
                    ProcessStorage.TryReloadRunningActivity(processData.Id, activityData);

                ActivityPool.AddActivity(processData, activityData);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e, "Activity creation for task {0} of process {1} with recipe {2} failed!",
                    taskTransition.Id, processData.Id, processData.Recipe.Id);
                NotificationAdapter.Publish(this, new Notification
                {
                    IsAcknowledgable = true,
                    Severity = Severity.Error,
                    Title = Strings.ActivityProvider_CreationException_Title,
                    Message = string.Format(Strings.ActivityProvider_CreationException_Message, taskTransition.Name, processData.Recipe.Name)
                }, processData);
            }
        }

        private ProcessData GetProcessByEngine(IWorkplanEngine engine)
        {
            var processContext = (ProcessWorkplanContext)engine.Context;
            return ActivityPool.GetProcess(processContext.Process);
        }

        #endregion

        /// <summary>
        /// Add workflow to collection thread safe
        /// </summary>
        private void PrepareEngine(ProcessData processData)
        {
            var workplan = ((IWorkplanRecipe)processData.Recipe).Workplan;
            processData.Engine = WorkplanInstance.CreateEngine(workplan, new ProcessWorkplanContext(processData.Process));

            processData.Engine.TransitionTriggered += OnTransitionTriggered;
            processData.Engine.Completed += OnWorkflowCompleted;
        }

        /// <summary>
        /// Remove workflow from memory and un-wire events
        /// </summary>
        private void RemoveWorkflow(ProcessData processData)
        {
            var workflow = processData.Engine;

            if (processData.State > ProcessState.Interrupted)
                DeleteHolders(processData); // Delete all remaining holders of the process

            workflow.TransitionTriggered -= OnTransitionTriggered;
            workflow.Completed -= OnWorkflowCompleted;

            workflow.Dispose();
        }

        /// <summary>
        /// Delete holders by loading the snapshot without using it
        /// </summary>
        private void DeleteHolders(ProcessData processData)
        {
            using var uow = UnitOfWorkFactory.Create();
            // Load holders from db
            var repo = uow.GetRepository<ITokenHolderEntityRepository>();
            var holders = repo.GetAllByProcessId(processData.Id);
            repo.RemoveRange(holders);

            uow.SaveChanges();
        }

        #region Notification Handling

        string INotificationSender.Identifier => NotificationSenderName;

        void INotificationSender.Acknowledge(Notification notification, object tag)
        {
            NotificationAdapter.Acknowledge(this, notification);
        }

        #endregion
    }
}

