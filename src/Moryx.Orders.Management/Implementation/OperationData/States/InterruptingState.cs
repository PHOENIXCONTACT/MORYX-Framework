// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InterruptingState), ResourceType = typeof(Strings))]
    internal sealed class InterruptingState : OperationDataStateBase
    {
        public override bool CanBegin => true;

        public override bool CanReduceAmount => true;

        public override bool CanAdvice => true;

        public override bool CanPartialReport => true;

        public override bool CanFinalReport => false;


        public InterruptingState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Interrupting)
        {
        }

        public override void IncreaseTargetBy(int amount, User user)
        {
            NextState(StateRunning);
            Context.HandleIncreaseTargetBy(amount);
        }

        public override void DecreaseTargetBy(int amount, User user)
            => Context.HandleDecreaseTargetBy(amount);

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


        public override void JobsUpdated(JobStateChangedEventArgs args)
        {
            if (args.CurrentState < JobClassification.Completed)
                return;

            if (Context.Operation.Jobs.All(j => j.Classification == JobClassification.Completed))
            {
                NextState(StateInterrupted);
                Context.HandleInterrupted();
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

        public override void Resume()
        {
            if (Context.Operation.Jobs.All(j => j.Classification == JobClassification.Completed))
            {
                NextState(StateInterrupted);
                Context.HandleInterrupted();
            }
        }
    }
}

