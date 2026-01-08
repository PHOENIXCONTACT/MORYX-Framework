// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Cells;
using Moryx.Logging;
using Moryx.Threading;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Default implementation of an IActivityDispatcher
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener), typeof(IActivityDispatcher))]
    internal sealed class ActivityDispatcher : IActivityPoolListener, IActivityDispatcher, IDisposable
    {
        #region Dependencies

        /// <summary>
        /// Pool full of activities waiting to be dispatched
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Resource management to push activities to resources
        /// </summary>
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Internal thread pool to decouple from resource manager events
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Logger to create feedback to administrators
        /// </summary>
        [UseChild(nameof(ActivityDispatcher))]
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Proxies of all cells the Activity Dispatcher works on
        /// </summary>
        private List<ICell> _cells = new();

        /// <summary>
        /// Sessions that were reported during ControlSystemSync or as RTW.Push but could not yet be processed
        /// </summary>
        private List<ResourceAndSession> _reportedSessions;

        /// <inheritdoc />
        public int StartOrder => 140;

        #endregion

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
            ActivityPool.ActivityChanged += OnActivityChanged;

            // Prepare list of ready to works with number of cells
            _reportedSessions = new List<ResourceAndSession>();
        }

        public void Start()
        {
            // Get all production cells
            _cells.AddRange(ResourceManagement.GetResources<ICell>());
            ResourceManagement.ResourceAdded += OnResourcesAdded;
            ResourceManagement.ResourceRemoved += OnResourceRemoved;

            // Iterate cells twice to register ALL events BEFORE informing them about the startup
            foreach (var cell in _cells)
            {
                // Register to events
                cell.ReadyToWork += OnCellReadyToWork;
                cell.NotReadyToWork += OnCellNotReadyToWork;
                cell.ActivityCompleted += OnCellActivityCompleted;
            }

            // AFTER all events are wired we can start informing them about the startup
            var sessions = new ResourceAndSession[_cells.Count][];
            Parallel.For(0, _cells.Count, index =>
            {
                var cell = _cells[index];
                try
                {
                    sessions[index] = cell.ProcessEngineAttached(new ProcessEngineContext())
                        .Select(session => new ResourceAndSession(cell, session))
                        .ToArray();
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "Failed to attach cell #{id} {name}.", cell.Id, cell.Name);
                    sessions[index] = [];
                }
            });

            // Process all sessions
            HandleAttachedCells(sessions.SelectMany(arr => arr));
        }

        public void Stop()
        {
            ResourceManagement.ResourceAdded -= OnResourcesAdded;
            ResourceManagement.ResourceRemoved -= OnResourceRemoved;

            // Inform resources we are shutting down and process their response
            foreach (var cell in _cells)
            {
                try
                {
                    foreach (var session in cell.ProcessEngineDetached().OfType<ActivityCompleted>())
                        HandleActivityCompleted(cell, session);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Shutting down process engine caused an exception in #{id} {name}", cell.Id, cell.Name);
                }
            }

            // Unregister from cell events
            foreach (var cell in _cells)
            {
                cell.ReadyToWork -= OnCellReadyToWork;
                cell.NotReadyToWork -= OnCellNotReadyToWork;
                cell.ActivityCompleted -= OnCellActivityCompleted;
            }
            _cells.Clear();
        }

        public void Dispose()
        {
            ActivityPool.ActivityChanged -= OnActivityChanged;
            ActivityPool.ProcessChanged -= OnProcessChanged;

            _reportedSessions = null;
        }

        public ResourceAndSession[] ExportSessions()
        {
            lock (_reportedSessions)
                return _reportedSessions.ToArray();
        }

        private void HandleAttachedCells(IEnumerable<ResourceAndSession> pairs)
        {
            foreach (var pair in pairs)
            {
                var cell = pair.Resource;
                var session = pair.Session;
                var processData = ActivityPool.GetProcess(session.Reference);
                // This process is not known to the activity pool, so we either ignore the sync or send an outfeed
                if (session.Reference.HasReference && processData == null)
                {
                    Logger.Log(LogLevel.Warning, "Received {0} for unknown process {1}!", session.GetType().Name, session.Reference);
                    if (session is ReadyToWork rtw)
                        Decouple(() => cell.SequenceCompleted(rtw.CompleteSequence(null, false)), nameof(ICell.SequenceCompleted), cell);
                    continue;
                }

                // If the process does not have activities yet
                // we store the reported session until everything is up and running
                if (processData is { State: < ProcessState.EngineStarted })
                {
                    lock (processData.ReportedSessions)
                        processData.ReportedSessions.Add(pair);
                    continue;
                }

                // All information complete, let's start
                switch (session)
                {
                    case ReadyToWork:
                        HandleReadyToWork(pair);
                        break;
                    case ActivityStart activityStart:
                        UpdateToRunning(activityStart);
                        break;
                    case ActivityCompleted completed:
                        HandleActivityCompleted(cell, completed);
                        break;
                }
            }
        }

        private void OnResourcesAdded(object sender, IResource e)
        {
            if (e is not ICell cell)
                return;

            if (_cells.Any(c => c.Id == cell.Id))
                return;

            _cells.Add(cell);

            // Register to events
            cell.ReadyToWork += OnCellReadyToWork;
            cell.NotReadyToWork += OnCellNotReadyToWork;
            cell.ActivityCompleted += OnCellActivityCompleted;

            // Attach the new cell
            var sessions = cell.ProcessEngineAttached(new ProcessEngineContext())
                .Select(s => new ResourceAndSession(cell, s));
            HandleAttachedCells(sessions);
        }

        private void OnResourceRemoved(object sender, IResource e)
        {
            var cell = e as ICell;
            if (cell == null)
                return;

            _cells.Remove(cell);

            // Unregister from events
            cell.ReadyToWork -= OnCellReadyToWork;
            cell.NotReadyToWork -= OnCellNotReadyToWork;
            cell.ActivityCompleted -= OnCellActivityCompleted;

            // Remove already reported session
            lock (_reportedSessions)
                _reportedSessions.RemoveAll(s => s.Resource.Equals(e));
        }

        private void UpdateToRunning(ActivityStart activityStart)
        {
            var activity = ActivityPool.GetByActivity(activityStart.Activity);
            if (activity != null)
                ActivityPool.UpdateActivity(activity, ActivityState.Running);
        }

        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            var activityData = args.ActivityData;
            var processData = activityData.ProcessData;

            switch (args.Trigger)
            {
                case ActivityState.Configured:
                    // Do not dispatch activities before the engine was flagged as started
                    if (processData.State >= ProcessState.EngineStarted // OR any activities are still processing results
                        && processData.Activities.All(a => a.State <= ActivityState.Running | a.State >= ActivityState.Completed))
                        HandleConfiguredActivity(activityData, processData);
                    break;
                case ActivityState.EngineProceeded:
                    // First complete the session
                    CompleteProcessOnCell(processData, (ICompletableSession)activityData.Session, activityData.Resource);
                    ActivityPool.UpdateActivity(activityData, ActivityState.Completed);

                    // NOW dispatch new activities
                    var configured = processData.Activities.Where(a => a.State == ActivityState.Configured);
                    foreach (var activity in configured)
                        HandleConfiguredActivity(activity, processData);
                    break;
            }

        }

        /// <summary>
        /// Handle a configured activity of a properly initialized process
        /// </summary>
        private void HandleConfiguredActivity(ActivityData activityData, ProcessData processData)
        {
            // Look if there is a ReadyToWork waiting
            ResourceAndSession match = null;
            var processRequirement = activityData.Activity.ProcessRequirement;
            var targetList = activityData.Targets.ToList();
            // If it must be empty we only look at empty reported sessions
            // Also NotRequired is provided by empty RTW in most cases
            if (processRequirement == ProcessRequirement.Empty || processRequirement == ProcessRequirement.NotRequired)
            {
                lock (_reportedSessions)
                {
                    match = _reportedSessions.Where(ras => ras.MatchesActivity(activityData))
                        .OrderBy(m => targetList.IndexOf(m.Resource)).FirstOrDefault();
                    _reportedSessions.Remove(match);
                }
            }
            // Required processes are only found in the process related sessions
            // Also see if we can find a match where the previous check did not
            if (processRequirement == ProcessRequirement.Required || processRequirement == ProcessRequirement.NotRequired && match == null)
            {
                lock (processData.ReportedSessions)
                {
                    match = processData.ReportedSessions.Where(ras => ras.MatchesActivity(activityData))
                        .OrderBy(m => targetList.IndexOf(m.Resource)).FirstOrDefault();
                    processData.ReportedSessions.Remove(match);
                }
            }

            // We found no match for the activity
            if (match == null)
                return;

            // Start the activity if we have a match AND it's a ready to work
            if (match.ReadyToWork != null)
                ParallelOperations.ExecuteParallel(() => StartActivity(match.Resource, match.ReadyToWork, activityData));
            else if (match.Session is ActivityStart)
                ActivityPool.UpdateActivity(activityData, ActivityState.Running);
            else if (match.Session is ActivityCompleted completedSession)
                HandleActivityCompleted(match.Resource, completedSession);
        }

        /// <summary>
        /// If a process changes to <see cref="ProcessState.Success"/> or <see cref="ProcessState.Failure"/>
        /// we remove all process related RTW.Push
        /// </summary>
        private void OnProcessChanged(object sender, ProcessEventArgs e)
        {
            // If the engine is ready or a broken process shall be removed,
            // see if we have cached sessions we can now respond to
            if (e.Trigger == ProcessState.EngineStarted || e.Trigger == ProcessState.RemoveBroken)
            {
                ProcessLoaded(e.ProcessData);
            }
            // Check if the process entered one of the Stopping/Removing states
            else if (e.Trigger == ProcessState.Aborting)
            {
                ProcessAborting(e.ProcessData);
            }
            else if (e.Trigger > ProcessState.Interrupted)
            {
                ProcessCompleted(e.ProcessData);
            }
        }

        /// <summary>
        /// Process was loaded and sessions should be responded to
        /// </summary>
        private void ProcessLoaded(ProcessData processData)
        {
            // Now that the process is loaded we can process its open activities
            var openActivities = ActivityPool.GetAllOpen(processData);
            foreach (var openActivity in openActivities)
                HandleConfiguredActivity(openActivity, processData);

            // The code above already takes care of all matching sessions, for all remaining
            // session we can send Finish OR SequenceCompleted
            lock (processData.ReportedSessions)
            {
                while (processData.ReportedSessions.Count > 0)
                {
                    var pair = processData.ReportedSessions[0];
                    var session = pair.Session;

                    switch (session)
                    {
                        case ReadyToWork rtw:
                            CompleteProcessOnCell(processData, rtw, pair.Resource);
                            break;
                        case ActivityCompleted completed:
                            HandleActivityCompleted(pair.Resource, completed);
                            break;
                    }
                    processData.ReportedSessions.Remove(pair);
                }
            }
        }

        /// <summary>
        /// Process is aborting and we should inform the cells currently executing the activity
        /// </summary>
        private void ProcessAborting(ProcessData processData)
        {
            // In case we have running activities we inform cells about the abortion
            var activities = ActivityPool.GetAllOpen(processData);
            foreach (var activity in activities)
            {
                if (activity.State <= ActivityState.Configured)
                {
                    // Activities that were not started can be aborted
                    ActivityPool.UpdateActivity(activity, ActivityState.Aborted);
                }
                else if (activity.State == ActivityState.Running)
                {
                    var cell = activity.Resource;
                    Logger.Log(LogLevel.Debug, "Aborting {0}-{1} in {2}-{3}!", activity.Id, activity.Activity.GetType().Name, cell.Id, cell.Name);
                    Decouple(() => cell.ProcessAborting(activity.Activity), nameof(ICell.ProcessAborting), cell);
                }
            }
        }

        /// <summary>
        /// A process was completed and we clean-up all related Sessions
        /// </summary>
        private void ProcessCompleted(ProcessData processData)
        {
            // A completed process might still have running activities, we should inform the cells
            if (processData.State >= ProcessState.Success)
                ProcessAborting(processData);

            // Complete and clear open sessions
            lock (processData.ReportedSessions)
            {
                foreach (var waitingRtw in processData.ReportedSessions.Where(rae => rae.ReadyToWork != null))
                {
                    var completed = waitingRtw.ReadyToWork.CompleteSequence(processData.Process, false);
                    Decouple(() => waitingRtw.Resource.SequenceCompleted(completed), nameof(ICell.SequenceCompleted), waitingRtw.Resource);
                }
                processData.ReportedSessions.Clear();
            }
        }

        private void OnCellReadyToWork(object sender, ReadyToWork message)
        {
            var cell = (ICell)sender;

            Logger.Log(LogLevel.Debug, "Received ReadyToWork from cell {0}-{1}. Type: {2} - Classification: {3} - Process: {4}",
                cell.Id, cell.Name, message.ReadyToWorkType, message.AcceptedClassification, message.Reference);

            ParallelOperations.ExecuteParallel(HandleReadyToWork, new ResourceAndSession(cell, message));
        }

        /// <summary>
        /// Handles the incoming <see cref="ReadyToWork"/> messages
        /// </summary>
        private void HandleReadyToWork(ResourceAndSession resourceAndSession)
        {
            // Extract cell and RTW session
            var cell = resourceAndSession.Resource;
            var message = resourceAndSession.ReadyToWork;

            // Get the process instance if it refers to a process
            var process = ActivityPool.GetProcess(message.Reference);
            // Use either the process related or general session cache
            var sessionCache = process?.ReportedSessions ?? _reportedSessions;

            ActivityData activity;
            lock (sessionCache)
            {
                // Remove duplicate sessions and only respond to the last one
                sessionCache.RemoveAll(resourceAndSession.Equals);

                // If found a match we can start now
                // Try to find matches from the activity pool
                var activities = ActivityPool.GetByCondition(resourceAndSession.MatchesActivity);
                if (activities.Count > 1)
                {
                    Logger.Log(LogLevel.Debug, "Found more than one matching activity for RTW from resource {0}-{1}. Picking running activity if possible!",
                        resourceAndSession.Resource.Id, resourceAndSession.Resource.Name);
                    activity = activities.FirstOrDefault(a => a.State == ActivityState.Running) ?? activities[0];
                }
                else
                {
                    // For 0 or 1 results this will select the activity or null
                    activity = activities.FirstOrDefault();
                }

                // Determine if we should queue the request and await an activity
                if (activity == null // No activity was found for the request
                    && (message.Reference.IsEmpty || process != null) // Either there was no process or it must be known
                    && (message.ReadyToWorkType == ReadyToWorkType.Push || process?.State < ProcessState.EngineStarted // Either RTW.PUSH or the process incompletely restored
                        || process?.State == ProcessState.Stopping))
                {
                    // Enqueue the new session to the appropriate cache
                    sessionCache.Add(resourceAndSession);
                    return;
                }
            }

            // If we found an activity, start it!
            if (activity != null)
            {
                StartActivity(cell, message, activity);
            }
            // Otherwise respond with finish. All conditions for queueing were handled above
            else
            {
                Logger.Log(LogLevel.Debug, "Sending outfeed to resource '{0}'", cell.Name);
                CompleteProcessOnCell(process, message, cell);
            }
        }

        /// <summary>
        /// Starts the activity
        /// </summary>
        private void StartActivity(ICell resource, ReadyToWork message, ActivityData activityData)
        {
            Logger.Log(LogLevel.Debug, "Dispatching activity '{0}' to resource '{1}'",
                activityData, resource.Id);

            // Always set the resource, even if this updates the current resource for redelivered activities
            activityData.Resource = resource;

            // Update activity if it wasn't started yet
            if (activityData.State < ActivityState.Running)
            {
                activityData.Activity.Tracing.Started = DateTime.Now;
                ActivityPool.UpdateActivity(activityData, ActivityState.Running);
            }
            // Update process if it wasn't started yet
            if (activityData.ProcessData.State < ProcessState.Running)
            {
                ActivityPool.UpdateProcess(activityData.ProcessData, ProcessState.Running);
            }

            // Start activity in cell after setting state
            var activityStart = message.StartActivity(activityData.Activity);
            activityData.Session = activityStart;
            Decouple(() => resource.StartActivity(activityStart), nameof(ICell.StartActivity), resource);
        }

        private void OnCellActivityCompleted(object sender, ActivityCompleted activityCompleted)
        {
            ParallelOperations.ExecuteParallel(HandleActivityCompleted, new ResourceAndSession((ICell)sender, activityCompleted));
        }

        /// <summary>
        /// Invocation target for parallel operations to improve Watchdog readability
        /// </summary>
        /// <param name="pair"></param>
        private void HandleActivityCompleted(ResourceAndSession pair) => HandleActivityCompleted(pair.Resource, (ActivityCompleted)pair.Session);

        /// <summary>
        /// The cell has completed the activity and reports the result
        /// </summary>
        private void HandleActivityCompleted(ICell resource, ActivityCompleted message)
        {
            var activity = ActivityPool.GetByActivity(message.CompletedActivity);

            if (activity is null)
                HandleForUnknownActivity(resource, message);
            else if (activity.State == ActivityState.Completed)
                HandleForAlreadyCompletedActivity(resource, message, activity);
            else if (activity.State >= ActivityState.ResultReceived)
                HandleForCurrentlyCompletingActivity(resource, message, activity);
            else if (message is UnknownActivityAborted)
                HandleForUnknownSession(resource, activity);
            else
                HandleActivityCompleted(resource, message, activity);
        }

        private void HandleForUnknownActivity(ICell resource, ActivityCompleted message)
        {
            Logger.Log(LogLevel.Warning, "Received result for unknown activity {activity}. Completing session immediately.", message.CompletedActivity);
            Decouple(() => resource.SequenceCompleted(message.CompleteSequence(message.CompletedActivity.Process, false)), nameof(ICell.SequenceCompleted), resource);
        }

        private void HandleForAlreadyCompletedActivity(ICell resource, ActivityCompleted message, ActivityData activity)
        {
            Logger.Log(LogLevel.Warning, "Received result {result} for already completed activity {activity} with result " +
                "{previousResult}. Completing session immediately.", message.CompletedActivity.Result.Numeric, activity, activity.Result.Numeric);
            Decouple(() => resource.SequenceCompleted(message.CompleteSequence(message.CompletedActivity.Process, false)), nameof(ICell.SequenceCompleted), resource);
        }

        private void HandleForCurrentlyCompletingActivity(ICell resource, ActivityCompleted message, ActivityData activity)
        {
            Logger.Log(LogLevel.Warning, "Dismissing received result {result} for currently completing activity {activity} with result " +
                "{previousResult}.", message.CompletedActivity.Result.Numeric, activity, activity.Result.Numeric);
        }

        private void HandleForUnknownSession(ICell resource, ActivityData activity)
        {
            Logger.Log(LogLevel.Warning, "{id}-{name} reported aborting of an unkown activity {activity}. " +
                                "Completing activity immediately.", resource.Id, resource.Name, activity);
            ActivityPool.UpdateActivity(activity, ActivityState.Completed);
        }

        private void HandleActivityCompleted(ICell resource, ActivityCompleted message, ActivityData activity)
        {
            Logger.Log(LogLevel.Debug, "Received result '{result}' for activity '{activity}'", message.CompletedActivity.Result.Numeric, activity);

            // Update session and resource
            activity.Session = message;
            activity.Resource = resource;

            // Update our activity from the reported one, just in case they are not identical
            activity.Result = message.CompletedActivity.Result;
            activity.Tracing = message.CompletedActivity.Tracing;
            activity.Tracing.Completed = DateTime.Now;

            ActivityPool.UpdateActivity(activity, ActivityState.ResultReceived);
        }

        /// <summary>
        /// Completes a sequence on a given cell
        /// </summary>
        private void CompleteProcessOnCell(ProcessData processData, ICompletableSession message, ICell cell)
        {
            if (processData is { State: < ProcessState.Interrupted })
            {
                var nextCells = processData.NextTargets().Select(c => c.Id).ToArray();
                var response = message.CompleteSequence(processData.Process, true, nextCells);
                Decouple(() => cell.SequenceCompleted(response), nameof(ICell.SequenceCompleted), cell);
            }
            else
            {
                var response = message.CompleteSequence(processData?.Process, false);
                Decouple(() => cell.SequenceCompleted(response), nameof(ICell.SequenceCompleted), cell);
            }
        }

        /// <summary>
        /// Cell is no longer <see cref="ReadyToWork"/> so it must be
        /// removed from the push message buffer
        /// </summary>
        private void OnCellNotReadyToWork(object sender, NotReadyToWork message)
        {
            var cell = (ICell)sender;

            Logger.Log(LogLevel.Debug, "Received NotReadyToWork from cell {0}-{1}. Classification: {2} - Process: {3}",
                cell.Id, cell.Name, message.AcceptedClassification, message.Reference);

            var sessionCache = GetSessionCache(message.Reference);
            lock (sessionCache)
                sessionCache.RemoveAll(resourceAndSession => resourceAndSession.ReadyToWork?.Id == message.Id);
        }

        /// <summary>
        /// Get the appropriate session cache depending on the process id
        /// </summary>
        private List<ResourceAndSession> GetSessionCache(ProcessReference processReference)
        {
            var process = ActivityPool.GetProcess(processReference);
            return process?.ReportedSessions ?? _reportedSessions;
        }

        /// <summary>
        /// Takes an <paramref name="actionToDispatch"/> that should be executed on the
        /// <paramref name="actionTarget"/> without exceptions feeding back into the process engine.
        /// </summary>
        private void Decouple(Action actionToDispatch, string actionName, ICell actionTarget)
        {
            // This makes sure, that exceptions from the cell can not discrupt the process engine. However, deadlocks or
            // very long running operations will still prevent e.g. process changes to complete. IMPORTANT: Pushing it to
            // a different thread without awaiting completion is not possible, as this will cause race conditions when
            // completing/removing activities.
            // ToDo: Apply timout already used when shutting down the process engine
            try
            {
                actionToDispatch();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An unhandled exception occured while executing {actionName} on #{id} {name}. " +
                    "The action is assumed to be completed.", actionName, actionTarget.Id, actionTarget.Name);
            }
        }
    }
}

