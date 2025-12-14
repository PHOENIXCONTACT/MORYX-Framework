// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Benchmarking;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.VisualInstructions;
using Moryx.Serialization;
using Moryx.Threading;

namespace Moryx.Resources.Benchmarking
{
    [ResourceRegistration]
    [DisplayName("Benchmark Reporter")]
    public class BenchmarkReporter : Cell
    {
        /// <summary>
        /// ParallelOperations for the reporting
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>
        /// Defines the interval of the reporting
        /// </summary>
        [DataMember, EntrySerialize, DefaultValue(1000)]
        public int ReportInterval { get; set; }

        /// <summary>
        /// Casted <see cref="Resource.Children"/> of this resource
        /// </summary>
        private IEnumerable<IBenchmarkResource> BenchmarkCells => Children.OfType<IBenchmarkResource>();

        /// <summary>
        /// Resource reference to a <see cref="IVisualInstructor"/> to show visual instructions
        /// </summary>
        [ResourceReference(ResourceRelationType.Extension)]
        [DisplayName("Visual Instructor")]
        public IVisualInstructor VisualInstructor { get; set; }

        private long _instructionId;
        private ActivityStart _currentSession;

        [DataMember, EntrySerialize] public int StepId { get; set; }

        /// <inheritdoc />
        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            Capabilities = new BenchmarkCapabilities(StepId);
        }

        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <inheritdoc />
        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await base.OnStartAsync(cancellationToken);

            ParallelOperations.ScheduleExecution(LogValues, ReportInterval, ReportInterval);
        }

        /// <inheritdoc />
        protected override IEnumerable<Session> ProcessEngineAttached()
        {
            yield return Session.StartSession(ActivityClassification.Setup, ReadyToWorkType.Push);
        }

        /// <inheritdoc />
        protected override IEnumerable<Session> ProcessEngineDetached()
        {
            yield break;
        }

        /// <inheritdoc />
        public override void StartActivity(ActivityStart activityStart)
        {
            _currentSession = activityStart;
            _instructionId = VisualInstructor.Execute(Name, activityStart, CompleteActivity);
            // Directly request a new activity
            var rtw = Session.StartSession(ActivityClassification.Setup, ReadyToWorkType.Push);
            PublishReadyToWork(rtw);
        }

        /// <inheritdoc />
        public override void ProcessAborting(Activity affectedActivity)
        {
            VisualInstructor.Clear(_instructionId);
            CompleteActivity(1, _currentSession);
        }

        /// <inheritdoc />
        public override void SequenceCompleted(SequenceCompleted completed)
        {
        }

        private void CompleteActivity(int result, ActivityStart activityStart)
        {
            var resultMsg = activityStart.CreateResult(result);
            PublishActivityCompleted(resultMsg);
        }

        private void LogValues()
        {
            var reports = BenchmarkCells.Select(r => r.GetReport()).Where(r => r.ActivityCount > 0).ToArray();
            if (reports.Length == 0)
                return; // Machine idle

            var total = reports.Sum(r => r.ActivityCount);
            var avgRtwWait = reports.Average(r => r.ReadyToWorkWait / r.ActivityCount);
            var avgAcWait = reports.Average(r => r.ActivityCompletionWait / r.ActivityCount);

            Logger.Log(LogLevel.Information,
                "Total: {0} activities/second - ReadyToWorkWait: {1:F2}ms - ActivityCompletedWait: {2:F2}ms", total,
                avgRtwWait, avgAcWait);
        }
    }
}
