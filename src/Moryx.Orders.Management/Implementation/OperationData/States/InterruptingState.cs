// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_InterruptingState), ResourceType = typeof(Strings))]
internal sealed class InterruptingState : OperationDataStateBase
{
    public override bool CanBegin => true;

    public override bool CanReduceAmount => true;

    public override bool CanAdvice => true;

    public override bool CanPartialReport => true;

    public override bool CanFinalReport => false;

    public InterruptingState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Interrupting)
    {
    }

    public override async Task IncreaseTargetBy(int amount, User user)
    {
        await NextStateAsync(StateRunning);
        await Context.HandleIncreaseTargetBy(amount);
    }

    public override Task DecreaseTargetBy(int amount, User user)
        => Context.HandleDecreaseTargetBy(amount);

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

    public override async Task JobsUpdated(JobStateChangedEventArgs args)
    {
        if (args.CurrentState < JobClassification.Completed)
            return;

        if (Context.Operation.Jobs.All(j => j.Classification == JobClassification.Completed))
        {
            await NextStateAsync(StateInterrupted);
            await Context.HandleInterrupted();
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

    public override async Task Resume()
    {
        if (Context.Operation.Jobs.All(j => j.Classification == JobClassification.Completed))
        {
            await NextStateAsync(StateInterrupted);
            await Context.HandleInterrupted();
        }
    }
}