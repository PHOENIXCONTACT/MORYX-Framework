// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Orders.Management.Properties;

namespace Moryx.Orders.Management
{
    [Display(Name = nameof(Strings.OperationState_CompletedState), ResourceType = typeof(Strings))]
    internal sealed class CompletedState : OperationDataStateBase
    {
        public CompletedState(OperationData context, StateMap stateMap)
            : base(context, stateMap, OperationStateClassification.Completed)
        {
        }

        public override bool CanAdvice => true;

        public override Task Resume()
        {
            return Task.CompletedTask;
        }

        public override Task Advice(OperationAdvice advice)
        {
            return Context.HandleAdvice(advice);
        }
    }
}

