﻿// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
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