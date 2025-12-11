// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Management.Properties;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_InitialAssignState), ResourceType = typeof(Strings))]
    internal sealed class InitialAssignState : OperationDataStateBase
    {
        public override bool IsAssigning => true;

        public InitialAssignState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Initial)
        {
        }

        public override Task Assign()
        {
            // Is already creating
            return Task.CompletedTask;
        }

        public override async Task AssignCompleted(bool success)
        {
            await Context.HandleAssignCompleted(success);
            await NextStateAsync(success ? StateReady : StateInitialAssignFailed);
        }

        public override Task Abort()
        {
            // cannot be aborted, but should also not throw an error
            return Task.CompletedTask;
        }
    }
}
