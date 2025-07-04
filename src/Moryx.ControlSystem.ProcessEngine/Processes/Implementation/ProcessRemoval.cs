﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Logging;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Makes sure that broken or removing processes are removed from the machine.
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IActivityPoolListener))]
    internal class ProcessRemoval : IActivityPoolListener, IDisposable
    {
        /// <summary>
        /// Reference to the pool every pool user needs
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Resource management to observer process changes
        /// </summary>
        public IResourceManagement ResourceManagement { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild(nameof(ProcessRemoval))]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Config that contains the removal instructions
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <inheritdoc />
        public int StartOrder => 80;

        /// <summary>
        /// Single reference for a dummy task to identify our activities
        /// </summary>
        private ITask _fixUpTask;

        public void Initialize()
        {
            ActivityPool.ProcessChanged += OnProcessChanged;
            ActivityPool.ActivityChanged += OnActivityChanged;

            // Register to process removal
            foreach (var removalCell in ResourceManagement.GetResources<IProcessReporter>())
            {
                removalCell.ProcessBroken += OnProcessBroken;
                removalCell.ProcessRemoved += OnProcessRemoved;
            }

            _fixUpTask = new ProcessFixUpTask(Config.RemovalMessage);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            // Register to process removal
            foreach (var removalCell in ResourceManagement.GetResources<IProcessReporter>())
            {
                removalCell.ProcessRemoved -= OnProcessRemoved;
                removalCell.ProcessBroken -= OnProcessBroken;
            }

            ActivityPool.ActivityChanged -= OnActivityChanged;
            ActivityPool.ProcessChanged -= OnProcessChanged;
        }

        /// <summary>
        /// Trigger on <see cref="ProcessState.Aborting"/> or <see cref="ProcessState.RemoveBroken"/>
        /// </summary>
        private void OnProcessChanged(object sender, ProcessEventArgs args)
        {
            switch (args.Trigger)
            {
                case ProcessState.Aborting:
                    CheckProcessAborted(args.ProcessData);
                    break;
                case ProcessState.RemoveBroken:
                    HandleRemoval(args.ProcessData);
                    break;
            }
        }

        /// <summary>
        /// UpdateList process when the unmount activity was completed
        /// </summary>
        private void OnActivityChanged(object sender, ActivityEventArgs args)
        {
            var activityData = args.ActivityData;
            var processData = activityData.ProcessData;
            // For fix up tasks we created, we need to update the activity to EngineProceeded,
            // after we set the process to failed
            if (args.Trigger == ActivityState.ResultReceived && activityData.Task == _fixUpTask)
            {
                // Removal was completed. We can put the process to rest
                ActivityPool.UpdateProcess(processData, ProcessState.Failure);
                ActivityPool.UpdateActivity(activityData, ActivityState.EngineProceeded);
            }
            // And for aborting processes we process the completed or aborted activities
            else if (processData.State == ProcessState.Aborting 
                && (args.Trigger == ActivityState.Completed || args.Trigger == ActivityState.Aborted))
            {
                CheckProcessAborted(processData);
            }                
        }

        private void CheckProcessAborted(ProcessData processData)
        {
            // If there are no more open activities, the process can be removed
            var openActivities = ActivityPool.GetAllOpen(processData);
            if (openActivities.Count == 0)
                ActivityPool.UpdateProcess(processData, ProcessState.RemoveBroken);
        }

        private void HandleRemoval(ProcessData processData)
        {
            // Check if an additional unmount is necessary
            if (RequiresUnmount(processData))
            {
                // Create unmount activity
                var unmount = (Activity)_fixUpTask.CreateActivity(processData.Process);
                var activityData = new ActivityData(unmount) { Task = _fixUpTask };
                ActivityPool.AddActivity(processData, activityData);
            }
            else if(processData.Activities.Any(a => a.EntityCreated | a.State >= ActivityState.Running))
            {
                // Fail process if there is any progress worth saving
                ActivityPool.UpdateProcess(processData, ProcessState.Failure);
            }
            else
            {
                // Otherwise simply discard it
                ActivityPool.UpdateProcess(processData, ProcessState.Discarded);
            }
        }

        /// <summary>
        /// A process failed outside of defined activity results, but remained in the machine
        /// </summary>
        private void OnProcessBroken(object sender, IProcess process)
        {
            var senderResource = (IResource)sender;
            Logger.Log(LogLevel.Warning, "Process {0} was reported as broken by {1}-{2}", process.Id, senderResource.Id, senderResource.Name);

            var processData = ActivityPool.GetProcess(process.Id);
            if (processData != null)
                ActivityPool.UpdateProcess(processData, ProcessState.Aborting);
        }

        /// <summary>
        /// A process was physically removed. Therefore all activities belonging to this process need to be aborted,
        /// and the process itself should be marked as a failure.
        /// </summary>
        private void OnProcessRemoved(object sender, IProcess process)
        {
            var senderResource = (IResource)sender;
            Logger.Log(LogLevel.Warning, "Process {0} was reported as failed by {1}-{2}", process?.Id, senderResource.Id, senderResource.Name);

            var processData = ActivityPool.GetProcess(process.Id);
            if (processData != null)
                ActivityPool.UpdateProcess(processData, ProcessState.Failure);
        }

        /// <summary>
        /// Last mounting activity that changed the value determines the current state
        /// </summary>
        private static bool RequiresUnmount(ProcessData process)
        {
            var lastAction = (from activity in process.Activities
                              where activity.State >= ActivityState.Completed && activity.Activity is IMountingActivity
                              let action = ((IMountingActivity)activity.Activity).Operation
                              where action != MountOperation.Unchanged
                              orderby activity.Tracing.Completed descending
                              select action).FirstOrDefault();
            return lastAction == MountOperation.Mount;
        }

        /// <summary>
        /// Alternative implementation of <see cref="ITask"/> for the process FixUp activity
        /// </summary>
        private class ProcessFixUpTask : ITask
        {
            public long Id => -1;

            public Type ActivityType => typeof(ProcessFixupActivity);

            private readonly VisualInstructionParameters _parameters;

            public ProcessFixUpTask(string removalMsg)
            {
                _parameters = new VisualInstructionParameters
                {
                    Instructions = new[]
                    {
                        new VisualInstruction
                        {
                            Type = InstructionContentType.Text,
                            Content = removalMsg
                        }, 
                    }
                };
            }

            public IActivity CreateActivity(IProcess process)
            {
                var activity = new ProcessFixupActivity
                {
                    Process = process,
                    Parameters = (VisualInstructionParameters)_parameters.Bind(process)
                };

                process.AddActivity(activity);

                return activity;
            }

            public void Completed(ActivityResult result)
            {
                // This method is not of interest for us
            }
        }
    }
}
