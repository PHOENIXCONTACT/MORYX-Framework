// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Pool listener that makes sure to provide articles for production processes and save modified articles
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener))]
    internal sealed class StorageTrigger : IActivityPoolListener, IDisposable
    {
        /// <summary>
        /// Injected activity pool
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Product management facade to save articles
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// ThreadPool do detach long taking operations from pool changes
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Injected process storage to load processes
        /// </summary>
        public IProcessStorage ProcessStorage { get; set; }

        /// <summary>
        /// Config for save instances flag
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        /// <summary>
        /// All processes that are currently created
        /// </summary>
        private readonly List<ProcessData> _creatingProcesses = new List<ProcessData>();

        /// <inheritdoc />
        public int StartOrder => 20;

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
            ActivityPool.ActivityChanged += OnActivityChanged;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            ActivityPool.ProcessChanged -= OnProcessChanged;
            ActivityPool.ActivityChanged -= OnActivityChanged;
        }

        /// <summary>
        /// When processes reach their finished state the articles need to be updated and saved
        /// </summary>
        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            var processData = args.ProcessData;
            switch (args.Trigger)
            {
                case ProcessState.Ready:
                    if (processData.Process is ProductionProcess prodProcess)
                        prodProcess.ProductInstance = ((IProductRecipe)prodProcess.Recipe).Target.CreateInstance();
                    break;
                case ProcessState.CleaningUp:
                case ProcessState.Restored:
                    if (processData.Process is ProductionProcess)
                        ParallelOperations.ExecuteParallel(LoadArticle, processData);
                    break;
                case ProcessState.Interrupted:
                    // Interrupted processes are saved synchronous for secure shutdown
                    ProcessStorage.SaveProcess(args.ProcessData);
                    // Save instances if existing and configured
                    if (ModuleConfig.SaveInstancesOnInterrupt && (processData.Process as ProductionProcess)?.ProductInstance?.Id > 0)
                        ParallelOperations.ExecuteParallel(SaveInstance, processData);
                    break;
                case ProcessState.Success:
                case ProcessState.Failure:
                    // Completed processes can be saved in parallel
                    ParallelOperations.ExecuteParallel(ProcessStorage.SaveProcess, args.ProcessData);
                    //Only update the instance if it exists and is not just a prepared object
                    if ((processData.Process as ProductionProcess)?.ProductInstance?.Id > 0)
                        ParallelOperations.ExecuteParallel(SaveInstance, processData);
                    break;
            }
        }

        /// <summary>
        /// Activities can modify the article and those changes need to be saved
        /// </summary>
        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            // Only completed activities can change an article
            if (args.Trigger != ActivityState.ResultReceived)
                return;

            var activityData = args.ActivityData;
            var processData = activityData.ProcessData;

            // We are only interested in IArticleModificationActivities on ProductionProcesses
            if (activityData.ProcessData.Process is ProductionProcess && activityData.Activity is IInstanceModificationActivity activity)
                ProcessArticleModification(activityData, processData, activity);

            SaveCompletedActivity(processData, activityData);

            ActivityPool.UpdateActivity(activityData, ActivityState.ResultProcessed);
        }

        /// <summary>
        /// Add a completed activity to the storage. Create the process if it was not created yet.
        /// Avoid race conditions with internal list of creating processes and fast spin-wait
        /// </summary>
        private void SaveCompletedActivity(ProcessData processData, ActivityData activityData)
        {
            // Spin wait
            while (!processData.EntityCreated)
            {
                lock (_creatingProcesses)
                {
                    // Someone else is creating/created the process
                    if (processData.EntityCreated || _creatingProcesses.Contains(processData))
                        continue;

                    // We create the process
                    _creatingProcesses.Add(processData);
                    break;
                }
            }

            try
            {
                // Write activity and (for first activity) process to database
                ProcessStorage.AddCompletedActivity(processData, activityData);
            }
            finally
            {
                lock (_creatingProcesses)
                {
                    _creatingProcesses.Remove(processData);
                }
            }
        }

        /// <summary>
        /// Process changes to the article
        /// </summary>
        private void ProcessArticleModification(ActivityData activityData, ProcessData processData, IInstanceModificationActivity activity)
        {
            // Only unsaved changes
            switch (activity.ModificationType)
            {
                case InstanceModificationType.Loaded:
                    if (activity.InstanceIdentity == null)
                    {
                        // If it was loaded, it exists physically
                        SaveInstance(processData);
                        // Consider this process a rework
                        processData.Rework = true;
                    }
                    else
                    {
                        // Replace prepared instance with existing one
                        LoadArticle(processData, activity.InstanceIdentity);
                    }
                    break;
                case InstanceModificationType.Created:
                case InstanceModificationType.Changed:
                    SaveInstance(activityData.ProcessData);
                    break;
            }
        }

        /// <summary>
        /// Load article by reference id
        /// </summary>
        private void LoadArticle(ProcessData processData)
        {
            var prodProcess = (ProductionProcess)processData.Process;
            if (processData.ReferenceId > 0) // Only load article if the process already has one!
                prodProcess.ProductInstance = ProductManagement.GetInstance(processData.ReferenceId);
            else // Otherwise provide a prepared, unsaved instance
                prodProcess.ProductInstance = ((IProductRecipe)prodProcess.Recipe).Target.CreateInstance();

            // Signal process ready for execution
            if (processData.State == ProcessState.Restored)
                ActivityPool.UpdateProcess(processData, ProcessState.RestoredReady);
            else if (processData.State == ProcessState.CleaningUp)
                ActivityPool.UpdateProcess(processData, ProcessState.CleaningUpReady);
        }

        /// <summary>
        /// Reload article instance by identifier and assign to process
        /// </summary>
        private void LoadArticle(ProcessData processData, IIdentity identity)
        {
            var productInstance = ProductManagement.GetInstance(identity);
            var prodProcess = (ProductionProcess)processData.Process;
            processData.Rework = productInstance.State == ProductInstanceState.Failure;
            prodProcess.ProductInstance = productInstance;
        }

        /// <summary>
        /// Save changes to the article
        /// </summary>
        private void SaveInstance(ProcessData processData)
        {
            // UpdateList article state and save
            var prodProcess = (ProductionProcess)processData.Process;
            var productInstance = prodProcess.ProductInstance;

            ProductManagement.SaveInstance(productInstance);

            // Update ReferenceId
            processData.ReferenceId = productInstance.Id;
        }
    }
}
