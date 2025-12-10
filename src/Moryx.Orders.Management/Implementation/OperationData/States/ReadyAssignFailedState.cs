// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_ReadyAssignFailedState), ResourceType = typeof(Strings))]
    internal sealed class ReadyAssignFailedState : OperationDataStateBase
    {
        public override bool CanAssign => true;

        public override bool IsFailed => true;

        public ReadyAssignFailedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Ready)
        {
        }

        public override async Task Assign()
        {
            await NextStateAsync(StateReadyAssign);
            Context.HandleReassign();
        }

        public override Task Resume()
        {
            return NextStateAsync(StateReady);
        }

        public override async Task Abort()
        {
            await NextStateAsync(StateAborted);
            await Context.HandleAbort();
        }
    }
}
