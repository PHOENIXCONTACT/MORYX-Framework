// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_ReadyAssignState), ResourceType = typeof(Strings))]
    internal sealed class ReadyAssignState : OperationDataStateBase
    {
        public override bool IsAssigning => true;

        public ReadyAssignState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationClassification.Ready)
        {
        }

        public override void AssignCompleted(bool success)
        {
            Context.HandleAssignCompleted(success);
            NextState(success ? StateReady : StateReadyAssignFailed);
        }

        public override void Resume()
        {
            NextState(StateReady);
        }

        public override void Abort()
        {
            // Cannot abort here
        }
    }
}
