// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    public class ActivityParameters : VisualInstructionParameters
    {

        protected override void Populate(IProcess process, Parameters instance)
        {
            var parameters = (ActivityParameters)instance;
        }

    }
}

