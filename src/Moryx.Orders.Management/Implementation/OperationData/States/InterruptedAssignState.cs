// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InterruptedAssignState), ResourceType = typeof(Strings))]
    internal sealed class InterruptedAssignState : OperationDataStateBase
    {
        public override bool IsAssigning => true;

        public InterruptedAssignState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Interrupted)
        {
        }

        public override void AssignCompleted(bool success)
        {
            Context.HandleAssignCompleted(success);
            NextState(success ? StateInterrupted : StateInterruptedAssignFailed);
        }

        public override void Resume()
        {
            NextState(StateInterrupted);
        }
    }
}
