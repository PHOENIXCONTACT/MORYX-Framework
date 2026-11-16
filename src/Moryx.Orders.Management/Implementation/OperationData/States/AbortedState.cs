// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management;

[Display(Name = nameof(Strings.OperationState_AbortedState), ResourceType = typeof(Strings))]
internal sealed class AbortedState : OperationDataStateBase
{
    public AbortedState(OperationData context, StateMap stateMap)
        : base(context, stateMap, OperationStateClassification.Aborted)
    {
    }
}
