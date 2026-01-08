// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_RunningState), ResourceType = typeof(Strings))]
    internal sealed class RunningState : OperationDataStateBase
    {
        public override bool CanBegin => true;

        public override bool CanReduceAmount => true;

        public override bool CanInterrupt => true;

        public override bool CanPartialReport => true;

        public override bool CanFinalReport => false;

        public override bool CanAdvice => true;

        public RunningState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Running)
        {
        }

        public override Task IncreaseTargetBy(int amount, User user)
            => Context.HandleIncreaseTargetBy(amount);

        public override Task DecreaseTargetBy(int amount, User user)
            => Context.HandleDecreaseTargetBy(amount);

        public override async Task Interrupt(User user)
        {
            await NextStateAsync(StateInterrupting);
            await Context.HandleManualInterrupting();
        }

        public override ReportContext GetReportContext()
        {
            return Context.HandleReportContext();
        }

        public override async Task Report(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Partial:
                    await Context.HandlePartialReport(report);
                    break;
                case ConfirmationType.Final:
                    // ReSharper disable once ExplicitCallerInfoArgument
                    await InvalidStateAsync(nameof(Report) + "(final)");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override AdviceContext GetAdviceContext()
        {
            return Context.HandleAdviceContext();
        }

        public override Task Advice(OperationAdvice advice)
        {
            return Context.HandleAdvice(advice);
        }

        public override Task JobsUpdated(JobStateChangedEventArgs args)
        {
            return EvaluateJobs();
        }

        public override Task ProgressChanged(Job job)
        {
            return EvaluateJobs();
        }

        public override async Task Resume()
        {
            await EvaluateJobs();
        }

        private async Task EvaluateJobs()
        {
            var jobs = Context.Operation.Jobs;
            var allCompleted = jobs.All(j => j.Classification == JobClassification.Completed);
            if (allCompleted && Context.AmountReached)
            {
                await NextStateAsync(StateAmountReached);
                return;
            }

            if (!Context.CanReachAmount)
            {
                await Context.DispatchJob();
            }
        }
    }
}

