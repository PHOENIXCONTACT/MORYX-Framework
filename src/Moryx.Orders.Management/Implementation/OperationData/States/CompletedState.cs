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
            : base(context, stateMap, OperationClassification.Completed)
        {
        }

        public override bool CanAdvice => true;

        public override void Resume()
        {
        }

        public override void Advice(OperationAdvice advice)
        {
            Context.HandleAdvice(advice);
        }
    }
}

