// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.Container;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Logging;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Implementation of <see cref="IActivityDataPool"/>
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityDataPool), typeof(IActivityPool))]
    internal sealed class ActivityPool : ILoggingComponent, IActivityDataPool, IActivityPool
    {
        #region Dependencies

        /// <summary>
        /// The freaking logger!!!!11elf
        /// </summary>
        [UseChild(nameof(ActivityPool))]
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// Processes currently managed by the pool
        /// </summary>
        private readonly List<ProcessData> _runningProcesses = new();

        /// <summary>
        /// List of non-completed activities for better scaling system
        /// </summary>
        private readonly List<ActivityData> _openActivities = new();

        /// <summary>
        /// Lock for the <see cref="_openActivities"/> collection
        /// </summary>
        private readonly ReaderWriterLockSlim _activitiesLock = new();

        #endregion

        public void AddProcess(ProcessData processData)
        {
            lock (_runningProcesses)
                _runningProcesses.Add(processData);

            RaiseProcessChanged(processData, processData.State);
        }

        public void AddActivity(ProcessData processData, ActivityData activityData)
        {
            activityData.ProcessData = processData;
            // Generate an id BEFORE adding to the pool
            if (activityData.Id == 0)
                activityData.Id = IdShiftGenerator.Generate(processData.Id, processData.ActivityIndex++);
            processData.AddActivity(activityData);

            _activitiesLock.EnterWriteLock();
            _openActivities.Add(activityData);
            _activitiesLock.ExitWriteLock();

            RaiseActivityChanged(activityData, activityData.State);
        }

        public void UpdateProcess(ProcessData processData, ProcessState newState)
        {
            if (newState <= processData.State)
            {
                Logger.Log(LogLevel.Warning, "Process states can only increase! Tried to change from '{0}' to '{1}'", processData.State, newState);
                return;
            }

            processData.State = newState;
            RaiseProcessChanged(processData, newState);

            // If process was completed or discarded it is removed from the list now
            if (processData.State >= ProcessState.Interrupted)
            {
                // Remove any pending activities
                _activitiesLock.EnterWriteLock();
                _openActivities.RemoveAll(a => a.ProcessData == processData);
                _activitiesLock.ExitWriteLock();

                // Remove the process
                lock (_runningProcesses)
                    _runningProcesses.Remove(processData);
            }
        }

        public void UpdateActivity(ActivityData activityData, ActivityState newState)
        {
            // Validate the change before performing it
            if (newState <= activityData.State)
            {
                Logger.Log(LogLevel.Warning, "States can only increase! Current state: {0} - New State: {1}", activityData.State, newState);
                return;
            }

            // Update the _openActivities collection
            if (newState is ActivityState.Completed or ActivityState.Aborted)
            {
                _activitiesLock.EnterWriteLock();
                _openActivities.Remove(activityData);
                _activitiesLock.ExitWriteLock();
            }

            // prevent state to be changed during enumeration
            _activitiesLock.EnterWriteLock();
            activityData.State = newState;
            _activitiesLock.ExitWriteLock();
            RaiseActivityChanged(activityData, newState);
        }

        private void RaiseProcessChanged(ProcessData processData, ProcessState trigger)
        {
            ProcessChanged(this, new ProcessEventArgs(processData, trigger));

            ProcessProgress progress;
            switch (trigger)
            {
                case ProcessState.Initial:
                case ProcessState.Restored:
                case ProcessState.CleaningUp:
                case ProcessState.Ready:
                case ProcessState.RestoredReady:
                    return; // Those states are too early to publish
                case ProcessState.EngineStarted:
                    progress = ProcessProgress.Ready;
                    break;
                case ProcessState.Running:
                case ProcessState.Aborting:
                case ProcessState.RemoveBroken:
                case ProcessState.Stopping:
                    progress = ProcessProgress.Running;
                    break;
                case ProcessState.Interrupted:
                case ProcessState.Discarded:
                    foreach (var activity in processData.Activities.Where(a => a.State <= ActivityState.Configured))
                        ActivityUpdated?.Invoke(this, new ActivityUpdatedEventArgs(activity.Activity, ActivityProgress.Completed));
                    progress = ProcessProgress.Interrupted;
                    break;
                case ProcessState.Success:
                case ProcessState.Failure:
                    progress = ProcessProgress.Completed;
                    break;
                default:
                    return;
            }

            ProcessUpdated?.Invoke(this, new ProcessUpdatedEventArgs(processData.Process, progress));
        }

        private void RaiseActivityChanged(ActivityData activityData, ActivityState trigger)
        {
            activityData.ProcessData.RaiseActivityChanged(activityData);
            ActivityChanged(this, new ActivityEventArgs(activityData, trigger));

            ActivityProgress progress;
            switch (trigger)
            {
                case ActivityState.Initial:
                case ActivityState.ResultReceived:
                    return; // The states above are either too early or too short-lived to publish
                case ActivityState.Configured:
                    progress = ActivityProgress.Ready;
                    break;
                case ActivityState.Running:
                    progress = ActivityProgress.Running;
                    break;
                case ActivityState.Aborted:
                case ActivityState.Completed:
                    progress = ActivityProgress.Completed;
                    break;
                default:
                    return;
            }

            ActivityUpdated?.Invoke(this, new ActivityUpdatedEventArgs(activityData.Activity, progress));
        }

        public ActivityData GetByActivity(IActivity wrapped)
        {
            // Performance optimized nested locking
            _activitiesLock.EnterReadLock();
            var result = _openActivities.FirstOrDefault(ad => ad.Activity == wrapped);
            _activitiesLock.ExitReadLock();

            return result;
        }

        public IReadOnlyList<ActivityData> GetByCondition(Func<ActivityData, bool> predicate)
        {
            // Performance optimized nested locking
            _activitiesLock.EnterReadLock();
            var result = _openActivities.Where(predicate).ToArray();
            _activitiesLock.ExitReadLock();

            return result;
        }

        public IReadOnlyList<ActivityData> GetAllOpen()
        {
            _activitiesLock.EnterReadLock();
            var result = _openActivities.ToArray();
            _activitiesLock.ExitReadLock();

            return result;
        }

        public IReadOnlyList<ActivityData> GetAllOpen(ProcessData processData)
        {
            return GetAllOpen(a => a.ProcessData == processData);
        }

        private IReadOnlyList<ActivityData> GetAllOpen(Func<ActivityData, bool> predicate)
        {
            _activitiesLock.EnterReadLock();
            var result = _openActivities.Where(predicate).ToArray();
            _activitiesLock.ExitReadLock();

            return result;
        }

        public int ProcessCount => _runningProcesses.Count;

        public IReadOnlyList<ProcessData> Processes
        {
            get
            {
                lock (_runningProcesses)
                {
                    return _runningProcesses.ToArray();
                }
            }
        }

        public ProcessData GetProcess(long id)
        {
            lock (_runningProcesses)
            {
                return _runningProcesses.FirstOrDefault(rp => rp.Id == id);
            }
        }

        public ProcessData GetProcess(ProcessReference reference)
        {
            if (reference.IsEmpty)
                return null;

            lock (_runningProcesses)
            {
                return _runningProcesses.FirstOrDefault(rp => reference.Matches(rp.Process));
            }
        }

        public ProcessData GetProcess(IProcess process)
        {
            lock (_runningProcesses)
            {
                return _runningProcesses.FirstOrDefault(rp => rp.Process == process);
            }
        }

        /// <summary>
        /// Do not check for null! Event must be registered!
        /// </summary>
        public event EventHandler<ProcessEventArgs> ProcessChanged;

        /// <summary>
        /// Do not check for null! Event must be registered!
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityChanged;

        #region IActivityPool
        IReadOnlyList<IProcess> IActivityPool.Processes
        {
            get
            {
                lock (_runningProcesses)
                {
                    return FastExtraction(_runningProcesses, pd => pd.Process);
                }
            }
        }

        IProcess IActivityPool.GetProcess(long id)
        {
            return GetProcess(id)?.Process;
        }

        public IReadOnlyList<IActivity> GetByCondition(Func<IActivity, bool> predicate)
        {
            var matches = GetByCondition(ad => predicate(ad.Activity));
            return FastExtraction(matches, ad => ad.Activity);
        }

        IReadOnlyList<IActivity> IActivityPool.GetAllOpen()
        {
            var open = GetAllOpen();
            return FastExtraction(open, ad => ad.Activity);
        }

        public IReadOnlyList<IActivity> GetAllOpen(IProcess process)
        {
            var wrapper = GetProcess(process);
            var open = GetAllOpen(wrapper);
            return FastExtraction(open, ad => ad.Activity);
        }

        public event EventHandler<ProcessUpdatedEventArgs> ProcessUpdated;

        public event EventHandler<ActivityUpdatedEventArgs> ActivityUpdated;
        #endregion

        /// <summary>
        /// Manual extraction is faster than Select and ToArray
        /// </summary>
        private static IReadOnlyList<TDomain> FastExtraction<TData, TDomain>(IReadOnlyList<TData> source, Func<TData, TDomain> extractor)
        {
            var extracted = new TDomain[source.Count];
            for (int i = 0; i < source.Count; i++)
                extracted[i] = extractor(source[i]);
            return extracted;
        }
    }
}
