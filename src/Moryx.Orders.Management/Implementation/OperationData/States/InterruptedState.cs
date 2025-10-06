// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InterruptedState), ResourceType = typeof(Strings))]
    internal sealed class InterruptedState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool CanBegin => true;

        public override bool CanPartialReport => false;

        public override bool CanFinalReport => true;

        public override bool CanAdvice => true;

        public InterruptedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Interrupted)
        {
        }

        public override ReportContext GetReportContext()
        {
            return Context.HandleReportContext();
        }

        public override void IncreaseTargetBy(int amount, User user)
        {
            // If partial amount is equal to zero we are instantly amount reached
            NextState(amount == 0 ? StateAmountReached : StateRunning);
            Context.HandleIncreaseTargetBy(amount);
            Context.HandleStarted(user);
        }

        public override void Report(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Final:
                    NextState(StateCompleted);
                    Context.HandleCompleted(report);
                    break;
                case ConfirmationType.Partial:
                    // ReSharper disable once ExplicitCallerInfoArgument
                    InvalidState(nameof(Report) + "(partial)");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(report.ConfirmationType), report.ConfirmationType, null);
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

        public override void Assign()
        {
            NextState(StateInterruptedAssign);
            Context.HandleReassign();
        }

        public override void Resume()
        {

        }
    }
}
