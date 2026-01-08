// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_ReadyAssignState), ResourceType = typeof(Strings))]
internal sealed class ReadyAssignState : OperationDataStateBase
{
    public override bool IsAssigning => true;

    public ReadyAssignState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Ready)
    {
    }

    public override async Task AssignCompleted(bool success)
    {
        await Context.HandleAssignCompleted(success);
        await NextStateAsync(success ? StateReady : StateReadyAssignFailed);
    }

    public override Task Resume()
    {
        return NextStateAsync(StateReady);
    }

    public override Task Abort()
    {
        // Cannot abort here
        return Task.CompletedTask;
    }
}