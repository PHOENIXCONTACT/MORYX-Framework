// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.Factory;
using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Activities;

namespace Moryx.FactoryMonitor.Endpoints.Tests.Resources;

public class DummyCell : Cell
{
    [EntryVisualization("celcius", "thermometer")]
    public double Temperature { get; set; }

    public void ChangeCapabilities(ICapabilities capabilities)
    {
        Capabilities = capabilities;
    }

    protected override IEnumerable<Session> ProcessEngineAttached()
    {
        yield break;
    }

    protected override IEnumerable<Session> ProcessEngineDetached()
    {
        yield break;
    }

    public override void StartActivity(ActivityStart activityStart)
    {
        throw new NotImplementedException();
    }

    public override void SequenceCompleted(SequenceCompleted completed)
    {
        throw new NotImplementedException();
    }
}