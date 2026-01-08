// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_AmountReachedAssignState), ResourceType = typeof(Strings))]
internal sealed class AmountReachedAssignState : OperationDataStateBase
{
    public override bool IsAssigning => true;

    public AmountReachedAssignState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Running)
    {
    }

    public override async Task AssignCompleted(bool success)
    {
        await Context.HandleAssignCompleted(success);
        await NextStateAsync(success ? StateAmountReached : StateAmountReachedAssignFailed);
    }

    public override Task Resume()
    {
        return NextStateAsync(StateAmountReached);
    }
}