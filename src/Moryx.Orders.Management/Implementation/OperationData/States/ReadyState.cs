// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_ReadyState), ResourceType = typeof(Strings))]
    internal sealed class ReadyState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool CanBegin => true;

        public override bool CanFinalReport => true;

        public override bool CanAdvice => true;

        public ReadyState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Ready)
        {
        }

        public override void IncreaseTargetBy(int amount, User user)
        {
            if (amount == 0)
            {
                NextState(StateAmountReached);
                Context.HandleIncreaseTargetBy(amount);
            }
            else
            {
                NextState(StateRunning);
                Context.HandleIncreaseTargetBy(amount);
            }

            Context.HandleStarted(user);
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
                    // ReSharper disable once ExplicitCallerInfoArgument
                    InvalidState(nameof(Report) + "(partial)");
                    break;
                case ConfirmationType.Final:
                    NextState(StateCompleted);
                    Context.HandleCompleted(report);
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

        public override void Resume()
        {

        }

        public override void Abort()
        {
            NextState(StateAborted);
            Context.HandleAbort();
        }

        public override void Assign()
        {
            NextState(StateReadyAssign);
            Context.HandleReassign();
        }
    }
}
