// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_AmountReachedState), ResourceType = typeof(Strings))]
    internal sealed class AmountReachedState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool CanBegin => true;

        public override bool CanPartialReport => true;

        public override bool CanFinalReport => true;

        public override bool CanInterrupt => true;

        public override bool CanAdvice => true;

        public override bool IsAmountReached => true;

        public AmountReachedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Running)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Context.ShowAmountReachedNotification();
        }

        public override void OnExit()
        {
            Context.AcknowledgeAmountReachedNotification();
            base.OnExit();
        }

        public override void IncreaseTargetBy(int amount, User user)
        {
            if (Context.ReachableAmount < Context.TargetAmount + amount)
            {
                NextState(StateRunning);
            }

            Context.HandleIncreaseTargetBy(amount);
        }

        public override void Interrupt(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Final:
                    NextState(StateCompleted);
                    Context.HandleCompleted(report);
                    break;
                case ConfirmationType.Partial:
                    NextState(StateInterrupted);
                    Context.HandleManualInterrupted(report);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(report.ConfirmationType), report.ConfirmationType, null);
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
                case ConfirmationType.Final:
                    NextState(StateCompleted);
                    Context.HandleCompleted(report);
                    break;
                case ConfirmationType.Partial:
                    Context.HandlePartialReport(report);
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
            NextState(StateAmountReachedAssign);
            Context.HandleReassign();
        }

        public override void Resume()
        {

        }

        public override void JobsUpdated(JobStateChangedEventArgs args)
        {
            var debug = $"Recieved a job update after switching to the '{nameof(AmountReachedState)}'. " +
                $"The update is ignored, as the operation has already recieved the latest values when switching states.";

            (Context as ILoggingComponent)?.Logger.Log(LogLevel.Debug, debug);
        }
    }
}

