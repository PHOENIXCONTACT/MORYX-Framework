// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.FactoryMonitor.Endpoints.Tests;

[ActivityResults(typeof(DefaultActivityResult))]
public class AssemblyActivity : Activity<ActivityParameters>
{
    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    public override ICapabilities RequiredCapabilities => new DummyCapabilities1();

    protected override ActivityResult CreateFailureResult()
    {
        return Fail();
    }

    protected override ActivityResult CreateResult(long resultNumber)
    {
        return Complete(resultNumber);
    }
}