// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
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

