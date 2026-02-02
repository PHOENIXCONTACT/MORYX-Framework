// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_InterruptedAssignFailedState), ResourceType = typeof(Strings))]
internal sealed class InterruptedAssignFailedState : OperationDataStateBase
{
    public override bool CanAssign => true;

    public override bool IsFailed => true;

    public InterruptedAssignFailedState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Interrupted)
    {
    }

    public override async Task Assign()
    {
        await NextStateAsync(StateInterruptedAssign);
        Context.HandleReassign();
    }

    public override Task Resume()
    {
        return NextStateAsync(StateInterrupted);
    }
}
