// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Activities;

namespace Moryx.ControlSystem.Tests
{
    public class DummyActivityParameters : Parameters, IActivityTimeoutParameters
    {
        public int Timeout { get; set; }

        protected override void Populate(IProcess process, Parameters instance)
        {
        }
    }
}