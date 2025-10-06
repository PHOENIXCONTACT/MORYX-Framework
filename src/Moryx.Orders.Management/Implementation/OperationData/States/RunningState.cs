// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
            : base(context, stateMap, OperationClassification.Running)
        {
        }

        public override void IncreaseTargetBy(int amount, User user)
            => Context.HandleIncreaseTargetBy(amount);

        public override void DecreaseTargetBy(int amount, User user)
            => Context.HandleDecreaseTargetBy(amount);

        public override void Interrupt(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Partial:
                    NextState(StateInterrupting);
                    Context.HandleManualInterrupting(report);
                    break;
                case ConfirmationType.Final:
                    // ReSharper disable once ExplicitCallerInfoArgument
                    InvalidState(nameof(Report) + "(final)");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override ReportContext GetReportContext()
        {
            return Context.HandleReportContext();
        }

        public override void Report(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Partial:
                    Context.HandlePartialReport(report);
                    break;
                case ConfirmationType.Final:
                    // ReSharper disable once ExplicitCallerInfoArgument
                    InvalidState(nameof(Report) + "(final)");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override AdviceContext GetAdviceContext()
        {
            return Context.HandleAdviceContext();
        }

        public override void Advice(OperationAdvice advice)
        {
            Context.HandleAdvice(advice);
        }

        public override void JobsUpdated(JobStateChangedEventArgs args)
        {
            EvaluateJobs();
        }

        public override void ProgressChanged(Job job)
        {
            EvaluateJobs();
        }

        public override void Resume()
        {
            EvaluateJobs();
        }

        private void EvaluateJobs()
        {
            var jobs = Context.Operation.Jobs;
            var allCompleted = jobs.All(j => j.Classification == JobClassification.Completed);
            if (allCompleted && Context.AmountReached)
            {
                NextState(StateAmountReached);
                return;
            }

            if (!Context.CanReachAmount)
            {
                Context.DispatchJob();
            }
        }
    }
}

