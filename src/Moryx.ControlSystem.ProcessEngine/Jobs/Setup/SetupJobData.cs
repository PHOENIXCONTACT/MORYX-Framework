// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.ProcessEngine.Properties;
using Moryx.ControlSystem.Recipes;
using Moryx.Notifications;
using Moryx.StateMachines;

namespace Moryx.ControlSystem.ProcessEngine.Jobs.Setup
{
    /// <summary>
    /// Implementation of a setup job
    /// </summary>
    [DebuggerDisplay(nameof(SetupJobData) + " <Id: {" + nameof(Id) + "}, State: {" + nameof(State) + "}>")]
    [Component(LifeCycle.Transient, typeof(ISetupJobData))]
    internal sealed class SetupJobData : JobDataBase, ISetupJobData, INotificationSender
    {
        #region Dependencies

        /// <summary>
        /// Configuration of the module
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        /// <summary>
        /// Adapter to publish notifications
        /// </summary>
        public INotificationAdapter NotificationAdapter { get; set; }

        #endregion

        /// <inheritdoc />
        public new SetupRecipe Recipe => (SetupRecipe)base.Recipe;

        /// <inheritdoc />
        public int RunningCount { get; private set; }

        /// <inheritdoc />
        public int CompletedCount { get; private set; }

        /// <summary>
        /// Override of the State to cast to correct type
        /// </summary>
        private new SetupJobStateBase State => (SetupJobStateBase)base.State;

        /// <inheritdoc />
        public bool RecipeRequired => State.RecipeRequired;

        /// <inheritdoc />
        public ProcessData ActiveProcess => RunningProcesses.FirstOrDefault();

        /// <summary>
        /// If dispatched, the setup process
        /// </summary>
        private ProcessData _setupProcess;

        /// <summary>
        /// How many time the setup was retried initially or since the the last user-acknowledgment
        /// </summary>
        private int _retryCounter;

        /// <summary>
        /// The Identifier used for retrieving notifications from the database
        /// </summary>
        private const string NotificationSenderName = "SetupJob";

        public SetupJobData(IWorkplanRecipe recipe) : base(recipe, recipe.Workplan.Steps.Count())
        {
            StateMachine.Initialize(this).With<SetupJobStateBase>();
        }

        public SetupJobData(IWorkplanRecipe recipe, JobEntity entity) : base(recipe, entity)
        {
            StateMachine.Reload(this, entity.State).With<SetupJobStateBase>();
        }

        public void UpdateSetup(SetupRecipe updatedRecipe)
        {
            InvokeStateMachine(s =>
            {
                ((SetupJobStateBase)s).UpdateSetup(updatedRecipe);
            });
        }

        internal void UpdateRecipe(IWorkplanRecipe recipe)
        {
            if (base.Recipe != null)
            {
                // resolve type of current disabled step
                foreach (var oldDisabledStepId in base.Recipe.DisabledSteps)
                {
                    // find task in current recipe 
                    var disabledStep = base.Recipe.Workplan.Steps.First(step => step.Id == oldDisabledStepId);
                    // find same task in new recipe
                    var step = recipe.Workplan.Steps.FirstOrDefault(step => step.GetType() == disabledStep.GetType());
                    // set disabled step in new recipe
                    if (step != null)
                        recipe.DisabledSteps.Add(step.Id);
                }
            }

            base.Recipe = recipe;
            Job.UpdateRecipe(recipe);
        }

        internal bool IsRetryLimitReached()
        {
            if (ModuleConfig.SetupJobRetryLimit < 0)
                return false;

            return _retryCounter > ModuleConfig.SetupJobRetryLimit;
        }

        internal void NotifyAboutBlockedRetry()
        {
            var notification = new Notification(Strings.SetupJobData_RetryFailedNotification_Title,
                string.Format(Strings.SetupJobData_RetryFailedNotification_Message, Recipe.Name), Severity.Error, true);

            NotificationAdapter.Publish(this, notification);
        }

        internal void ClearNotification()
        {
            _retryCounter = 0;
            NotificationAdapter.AcknowledgeAll(this);
        }

        /// <inheritdoc />
        public override void AddProcess(ProcessData processData)
        {
            _setupProcess = processData;

            // Register to changes to get activity updates
            processData.ActivityChanged += OnActivityChanged;

            base.AddProcess(processData);

            Job.Running.Add(processData.Process);
        }

        /// <inheritdoc />
        public override void FailurePredicted(ProcessData processData)
        {
            // We do not care about predicted failures
        }

        /// <inheritdoc />
        internal override void ProcessCompleted(ProcessData processData)
        {
            processData.ActivityChanged -= OnActivityChanged;
            _setupProcess = null;

            RunningProcesses.Remove(processData);

            base.ProcessCompleted(processData);
        }

        /// <summary>
        /// Increment retry counter
        /// </summary>
        internal void IncrementRetry() => _retryCounter++;

        /// <summary>
        /// Listen to activity changes to handle the progress of this setup job
        /// </summary>
        private void OnActivityChanged(object sender, ActivityData activityData)
        {
            CompletedCount = _setupProcess.ActivityCount(a =>
                a.State == ActivityState.Aborted ||
                a.State == ActivityState.Completed);

            RunningCount = _setupProcess.ActivityCount(a =>
                a.State == ActivityState.Running ||
                a.State == ActivityState.ResultReceived);

            // Only publish progress changed while we execute something
            if (RunningCount > 0)
                RaiseProgressChanged();
        }

        /// <inheritdoc/>
        public void Acknowledge(Notification notification, object tag)
        {
            InvokeStateMachine(s => ((SetupJobStateBase)s).UnBlockRetrySetup());
        }

        /// <inheritdoc/>
        public string Identifier => NotificationSenderName;
    }
}

