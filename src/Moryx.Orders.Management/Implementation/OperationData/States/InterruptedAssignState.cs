// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_InterruptedAssignState), ResourceType = typeof(Strings))]
internal sealed class InterruptedAssignState : OperationDataStateBase
{
    public override bool IsAssigning => true;

    public InterruptedAssignState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Interrupted)
    {
    }

    public override async Task AssignCompleted(bool success)
    {
        await Context.HandleAssignCompleted(success);
        await NextStateAsync(success ? StateInterrupted : StateInterruptedAssignFailed);
    }

    public override Task Resume()
    {
        return NextStateAsync(StateInterrupted);
    }
}
