// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.Benchmarking;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.VisualInstructions;
using Moryx.Notifications;
using Moryx.Serialization;

namespace Moryx.Resources.Benchmarking
{
    [ResourceRegistration]
    [DisplayName("Benchmark Resource")]
    public class BenchmarkResource : Cell, IBenchmarkResource, INotificationSender

    {
        #region Config

        /// <summary>
        /// Flag that processes shall be confirmed manually
        /// </summary>
        [DataMember, EntrySerialize]
        public bool ManualProcessConfirmation { get; set; }

        /// <summary>
        /// Additional configured visual instructions for this resource
        /// </summary>
        [DataMember, EntrySerialize]
        public VisualInstruction[] Instructions { get; set; }

        #endregion

        #region Dependencies

        public INotificationAdapter NotificationAdapter { get; set; }

        [ResourceReference(ResourceRelationType.Extension, IsRequired = true)]
        [DisplayName("Visual Instructor")]
        [Description("Target to show instructions while benchmarking")]
        public IVisualInstructor VisualInstructor { get; set; }

        /// <summary>
        /// Additional configured possible results for the visual instructions for this resource
        /// </summary>
        [DataMember, EntrySerialize]
        public InstructionResult[] PossibleResults { get; set; }

        #endregion

        private long _instructionId;

        private int _activityCount;
        private readonly Stopwatch _rtwWait = new();
        private readonly Stopwatch _acWait = new();
        private readonly Stopwatch _runtimeWait = new();

        /// <summary>
        /// Step id of this resource
        /// </summary>
        [DataMember, EntrySerialize]
        public int StepId { get; set; }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Capabilities = new BenchmarkCapabilities(StepId);
        }

        /// <inheritdoc />
        protected override IEnumerable<Session> ProcessEngineAttached()
        {
            yield return Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
        }

        /// <inheritdoc />
        protected override IEnumerable<Session> ProcessEngineDetached()
        {
            yield break;
        }

        public override void StartActivity(ActivityStart activityStart)
        {
            _rtwWait.Stop();
            _runtimeWait.Start();

            if (ManualProcessConfirmation)
            {
                var parameters = (BenchmarkParameters)activityStart.Activity.Parameters;

                var preparedInstructions = parameters.Instructions.Concat(Instructions);

                var instructionResolver = new VisualInstructionBinder(preparedInstructions, new ProcessBindingResolverFactory());
                var resolved = instructionResolver.ResolveInstructions(activityStart.Process);

                _instructionId = VisualInstructor.Execute(Name, activityStart, CompleteActivity, resolved);
            }
            else
            {
                CompleteActivity(0, activityStart);
            }
        }

        /// <inheritdoc />
        public override void ProcessAborting(IActivity affectedActivity)
        {
            VisualInstructor.Clear(_instructionId);
        }

        private void CompleteActivity(int result, ActivityStart activityStart)
        {
            _runtimeWait.Stop();

            var activity = activityStart.Activity;
            var tracing = (BenchmarkTracing)activity.Tracing;
            tracing.RuntimeMs = _runtimeWait.ElapsedMilliseconds;

            _runtimeWait.Reset();

            activity.Complete(result);

            var resultMsg = activityStart.CreateResult();

            _acWait.Start();

            // ReSharper disable once PossibleNullReferenceException
            PublishActivityCompleted(resultMsg);
        }

        /// <inheritdoc />
        public override void SequenceCompleted(SequenceCompleted completed)
        {
            _acWait.Stop();
            _activityCount++;

            _rtwWait.Start();
            var rtw = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
            PublishReadyToWork(rtw);
        }

        /// <inheritdoc />
        public BenchmarkReport GetReport()
        {
            var report = new BenchmarkReport
            {
                ActivityCount = _activityCount,
                ReadyToWorkWait = _rtwWait.ElapsedMilliseconds,
                ActivityCompletionWait = _acWait.ElapsedMilliseconds
            };
            _activityCount = 0;
            _rtwWait.Reset();
            _acWait.Reset();
            return report;
        }

        [DisplayName("Change Capabilities")]
        [EntrySerialize, Description("Change capabilities of the cell")]
        public void ChangeCapabilities([Description("New step value for the capabilities. '0' resets to LocalIdentifier")] int stepId = 0)
        {
            Capabilities = new BenchmarkCapabilities(stepId == 0 ? StepId : stepId);
        }

        [EntrySerialize]
        public void PublishNotification(string title, string message, Severity severity, bool isAcknowledgable)
        {
            var notification = new Notification(title, message, severity, isAcknowledgable);
            NotificationAdapter.Publish(this, notification);
        }

        [EntrySerialize]
        public void CreateAssemblyInstruction()
        {
            _instructionId = VisualInstructor.Execute(new ActiveInstruction
            {
                Title = Name,
                Instructions = Instructions,
                Results = PossibleResults,
            }, response => { });
        }

        [EntrySerialize]
        public void ClearAssemblyInstruction()
        {
            VisualInstructor.Clear(_instructionId);
        }

        [EntrySerialize]
        [DisplayName("Acknowledge last notification")]
        public void AcknowledgeLast()
        {
            var notifications = NotificationAdapter.GetPublished(this);
            if (notifications.Any())
                NotificationAdapter.Acknowledge(this, notifications.Last());
        }

        [EntrySerialize]
        [DisplayName("Acknowledge all notifications")]
        public void AcknowledgeAll()
        {
            NotificationAdapter.AcknowledgeAll(this);
        }

        void INotificationSender.Acknowledge(Notification notification, object tag)
        {
            NotificationAdapter.Acknowledge(this, notification);
        }

        public void Completed(long id, string result)
        {
            throw new NotImplementedException();
        }

        string INotificationSender.Identifier => Id.ToString();
    }
}

