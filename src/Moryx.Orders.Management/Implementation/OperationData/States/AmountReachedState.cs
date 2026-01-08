// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
            : base(context, stateMap, OperationStateClassification.Running)
        {
        }

        public override async Task IncreaseTargetBy(int amount, User user)
        {
            if (Context.ReachableAmount < Context.TargetAmount + amount)
            {
                await NextStateAsync(StateRunning);
            }

            await Context.HandleIncreaseTargetBy(amount);
        }

        public override async Task Interrupt(User user)
        {
            await NextStateAsync(StateInterrupted);
            await Context.HandleManualInterrupted();
        }

        public override ReportContext GetReportContext()
        {
            return Context.HandleReportContext();
        }

        public override async Task Report(OperationReport report)
        {
            switch (report.ConfirmationType)
            {
                case ConfirmationType.Final:
                    await NextStateAsync(StateCompleted);
                    await Context.HandleCompleted(report);
                    break;
                case ConfirmationType.Partial:
                    await Context.HandlePartialReport(report);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(report.ConfirmationType), report.ConfirmationType, null);
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

        public override async Task Assign()
        {
            await NextStateAsync(StateAmountReachedAssign);
            Context.HandleReassign();
        }

        public override Task Resume()
        {
            return Task.CompletedTask;
        }

        public override Task JobsUpdated(JobStateChangedEventArgs args)
        {
            var debug = $"Recieved a job update after switching to the '{nameof(AmountReachedState)}'. " +
                $"The update is ignored, as the operation has already recieved the latest values when switching states.";

            (Context as ILoggingComponent)?.Logger.Log(LogLevel.Debug, debug);

            return Task.CompletedTask;
        }
    }
}

