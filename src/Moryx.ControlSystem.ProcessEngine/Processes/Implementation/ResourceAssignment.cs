// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.Logging;
using Moryx.Notifications;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Pool listener that assigns the target resource to an activity and updates the
    /// state to <see cref="ActivityState.Configured"/>
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener), typeof(IResourceAssignment))]
    internal sealed class ResourceAssignment : IActivityPoolListener, IResourceAssignment, ILoggingComponent, INotificationSender, IDisposable
    {
        /// <summary>
        /// Identifier for notifications send by the ResourceAssignment
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const string NotificationSenderName = nameof(ResourceAssignment);

        #region Dependencies

        /// <summary>
        /// Logger for error feedback
        /// </summary>
        [UseChild(nameof(ResourceAssignment))]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// The very cool ActivityPool
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Resource management to determine the correct resource
        /// </summary>
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Used to handle notifications
        /// </summary>
        public INotificationAdapter NotificationAdapter { get; set; }

        /// <summary>
        /// Factory to instantiate <see cref="ICellSelector"/>
        /// </summary>
        public ICellSelectorFactory SelectorFactory { get; set; }

        /// <summary>
        /// Module configuration to configure notification content
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        #endregion

        /// <inheritdoc/>
        public int StartOrder => 60;

        /// <inheritdoc/>
        public string Identifier => NotificationSenderName;

        /// <summary>
        /// Configured selectors
        /// </summary>
        private List<ICellSelector> _selectors;

        /// <inheritdoc/>
        public void Initialize()
        {
            _selectors = ModuleConfig.ResourceSelectors
                .OrderBy(c => c.SortOrder)
                .Select(SelectorFactory.Create).ToList();

            ActivityPool.ActivityChanged += OnActivityAdded;
            ActivityPool.ProcessChanged += OnProcessChanged;

            ResourceManagement.ResourceAdded += OnResourceAdded;
            ResourceManagement.CapabilitiesChanged += OnCapabilitiesChanged;
            ResourceManagement.ResourceRemoved += OnResourceRemoved;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _selectors.ForEach(s => s.StartAsync().Wait());
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _selectors.ForEach(Stop);
        }

        public void Stop(ICellSelector selector)
        {
            try
            {
                selector.StopAsync().Wait();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "{selector} threw an exception while shutting down the process engine", selector.GetType().Name);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ResourceManagement.ResourceRemoved -= OnResourceRemoved;
            ResourceManagement.CapabilitiesChanged -= OnCapabilitiesChanged;
            ResourceManagement.ResourceAdded -= OnResourceAdded;

            ActivityPool.ProcessChanged -= OnProcessChanged;
            ActivityPool.ActivityChanged -= OnActivityAdded;
        }

        /// <summary>
        /// An activity was added to the pool and needs to be assigned to a resource
        /// </summary>
        private void OnActivityAdded(object sender, ActivityEventArgs args)
        {
            var activityData = args.ActivityData;
            if (args.Trigger == ActivityState.Initial)
                AssignAndUpdateActivity(activityData);
            else if (args.Trigger == ActivityState.ResultReceived)
                activityData.Targets = ActivityData.EmptyTargets;
        }

        private void OnProcessChanged(object sender, ProcessEventArgs e)
        {
            if (e.Trigger == ProcessState.EngineStarted)
            {
                // Restore cell on completed activities
                foreach (var completedActivity in e.ProcessData.Activities.Where(a => a.State == ActivityState.Completed))
                    completedActivity.Resource = ResourceManagement.GetResource<ICell>(completedActivity.Resource.Id);
            }
            else if (e.Trigger >= ProcessState.Interrupted)
            {
                // When the process is removed, we acknowledge all related Unassigned notifications
                e.ProcessData.Activities.ForEach(a => NotificationAdapter.AcknowledgeAll(this, a));
            }
        }

        private void AssignAndUpdateActivity(ActivityData activityData)
        {
            var cells = ResourceManagement.GetResources<ICell>(activityData.RequiredCapabilities).ToList();

            // Pass cells through selectors
            var selectedCells = SelectResources(activityData, cells);

            // Try to fetch a matching resource
            if (selectedCells.Count == 0)
            {
                //No matching resource => no activity update. Notify the user instead.
                PublishUnassignedActivity(activityData);
                return;
            }

            Logger.Log(LogLevel.Debug, "Activity '{0}' assigned to '{1}'", activityData, string.Join(",", selectedCells.Select(c => c.Id)));
            activityData.Targets = selectedCells;
            ActivityPool.UpdateActivity(activityData, ActivityState.Configured);
            NotificationAdapter.AcknowledgeAll(this, activityData);
        }

        /// <summary>
        /// A new resource was added, check if it provides capabilities for an open activity
        /// </summary>
        private void OnResourceAdded(object sender, IResource resource)
        {
            OnCapabilitiesChanged(resource, resource.Capabilities);
        }

        /// <summary>
        /// Capabilities of a resource have changed and all configured activities need to be reevaluated
        /// </summary>
        private void OnCapabilitiesChanged(object sender, ICapabilities changedCapabilities)
        {
            // Check if the sender was a sell
            if (sender is not ICell cell)
                return;

            // Update possible resources for non-running activities
            var openActivities = ActivityPool.GetAllOpen();
            foreach (var activityData in openActivities.Where(ad => ad.State <= ActivityState.Running))
            {
                // Check if the modified capabilities match those of the activity
                var provided = activityData.RequiredCapabilities.ProvidedBy(changedCapabilities);
                var contains = activityData.Targets.Contains(cell);

                // Both true or both false
                if (provided == contains)
                {
                    // The resource either already provided the capabilities or never has and never will
                    continue;
                }

                // Reassign activity
                var cells = ResourceManagement.GetResources<ICell>(activityData.RequiredCapabilities).ToList();
                var updatedTargets = SelectResources(activityData, cells);

                // Count available targets
                var previousTargetsCount = activityData.Targets.Count;
                var updatedTargetsCount = updatedTargets.Count;

                // Update the targets for the activity
                activityData.Targets = updatedTargets;

                // See if this caused or resolved unassigned activity
                if (previousTargetsCount == 0 && updatedTargetsCount > 0)
                {
                    // Acknowledge any existing Unassigned notification if there are possible targets
                    ActivityPool.UpdateActivity(activityData, ActivityState.Configured);
                    NotificationAdapter.AcknowledgeAll(this, activityData);
                }
                else if (previousTargetsCount > 0 && updatedTargetsCount == 0)
                {
                    PublishUnassignedActivity(activityData);
                }
            }

            // Inform others about the change
            CapabilitiesChanged?.Invoke(this, changedCapabilities);
        }

        /// <summary>
        /// Pass possible resources through selectors
        /// </summary>
        private IReadOnlyList<ICell> SelectResources(ActivityData activityData, IReadOnlyList<ICell> possibleResources)
        {
            // Pass possible cells through selectors
            var selectedCells = possibleResources;
            foreach (var selector in _selectors)
            {
                var selectionIteration = selector.SelectCellsAsync(activityData.Activity, selectedCells).GetAwaiter().GetResult();
                // Validate selection
                if (selectionIteration.Any(c => !possibleResources.Contains(c)))
                {
                    Logger.Log(LogLevel.Error, "Invalid resource selection for {0} by {1}: Cells must not be added!",
                        activityData.GetType().Name, selector.GetType().Name);
                }
                else
                {
                    selectedCells = selectionIteration;
                }
            }

            return selectedCells;
        }

        /// <summary>
        /// A Resource has been removed and all configured activities need to be reevaluated
        /// </summary>
        private void OnResourceRemoved(object sender, IResource resource)
        {
            // Check if the sender was a sell
            if (resource is not ICell cell)
                return;

            // Update possible resources for non-running activities
            var openActivities = ActivityPool.GetAllOpen();
            foreach (var activityData in openActivities.Where(ad => ad.State <= ActivityState.Running && ad.Targets.Contains(cell)))
            {
                // Reassign activity
                var cells = ResourceManagement.GetResources<ICell>(activityData.RequiredCapabilities).ToList();
                var updatedTargets = SelectResources(activityData, cells);

                // See if this caused or resolved unassigned activity
                if (activityData.Targets.Count > 0 && updatedTargets.Count == 0)
                {
                    PublishUnassignedActivity(activityData);
                }

                // Update the targets for the activity
                activityData.Targets = updatedTargets;
            }
        }

        /// <inheritdoc/>
        public void Acknowledge(Notification notification, object tag)
        {
            var activityData = tag as ActivityData;
            if (activityData == null)
                return;

            // Check if activity has a target before accepting the acknowledgement
            if (activityData.Resource is null // Check whether assignement was already successful
                && activityData.Targets.Count == 0)  // Check whether assignement could now be done successfully
                return;

            NotificationAdapter.Acknowledge(this, notification);
        }

        private void PublishUnassignedActivity(ActivityData activityData)
        {
            //No matching resource => Notify the user
            Logger.Log(LogLevel.Error, "No resource has required capabilities for '{0}'", activityData.Activity.GetType().Name);

            var notification = new Notification(Strings.ResourceAssignment_UnassignedActivityNotification_Title,
                string.Format(Strings.ResourceAssignment_UnassignedActivityNotification_Message, activityData.Activity.GetType().Name), ModuleConfig.UnassignedActivitySeverity, true);

            NotificationAdapter.Publish(this, notification, activityData);
        }

        public event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}
