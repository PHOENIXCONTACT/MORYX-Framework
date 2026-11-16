// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_AmountReachedAssignFailedState), ResourceType = typeof(Strings))]
internal sealed class AmountReachedAssignFailedState : OperationDataStateBase
{
    public override bool CanAssign => true;

    public override bool IsFailed => true;

    public AmountReachedAssignFailedState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Running)
    {
    }

    public override async Task Assign()
    {
        await NextStateAsync(StateAmountReachedAssign);
        Context.HandleReassign();
    }

    public override Task Resume()
    {
        return NextStateAsync(StateAmountReached);
    }
}
