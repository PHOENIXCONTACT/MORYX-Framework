// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;
using Moryx.Users;

namespace Moryx.Orders.Management;

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

    public override async Task IncreaseTargetBy(int amount, User user)
    {
        await NextStateAsync(amount == 0 ? StateAmountReached : StateRunning);

        await Context.HandleIncreaseTargetBy(amount);

        Context.HandleStarted(user);
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
                // ReSharper disable once ExplicitCallerInfoArgument
                await InvalidStateAsync(nameof(Report) + "(partial)");
                break;
            case ConfirmationType.Final:
                await NextStateAsync(StateCompleted);
                await Context.HandleCompleted(report);
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

    public override Task Resume()
    {
        return Task.CompletedTask;
    }

    public override async Task Abort()
    {
        await NextStateAsync(StateAborted);
        await Context.HandleAbort();
    }

    public override async Task Assign()
    {
        await NextStateAsync(StateReadyAssign);
        Context.HandleReassign();
    }
}