// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_AmountReachedAssignFailedState), ResourceType = typeof(Strings))]
    internal sealed class AmountReachedAssignFailedState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool IsFailed => true;

        public AmountReachedAssignFailedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Running)
        {
        }

        public override void Assign()
        {
            NextState(StateAmountReachedAssign);
            Context.HandleReassign();
        }

        public override void Resume()
        {
            NextState(StateAmountReached);
        }
    }
}
