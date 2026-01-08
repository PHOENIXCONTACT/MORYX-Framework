// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InitialAssignFailedState), ResourceType = typeof(Strings))]
    internal sealed class InitialAssignFailedState : OperationDataStateBase
    {
        public override bool IsFailed => true;

        public override bool CanAssign => true;

        public override bool IsCreated => false;

        public InitialAssignFailedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Initial)
        {
        }

        public override async Task Assign()
        {
            await NextStateAsync(StateInitialAssign);
            Context.HandleAssign();
        }

        public override async Task Abort()
        {
            await NextStateAsync(StateAborted);
            await Context.HandleAbort();
        }
    }
}
