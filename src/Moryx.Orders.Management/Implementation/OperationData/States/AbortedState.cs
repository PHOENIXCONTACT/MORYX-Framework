// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_AbortedState), ResourceType = typeof(Strings))]
    internal sealed class AbortedState : OperationDataStateBase
    {
        public AbortedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Aborted)
        {
        }
    }
}
