// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Tests.TestData;

public class OtherTestActivity : Activity<NullActivityParameters>
{
    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    public override ICapabilities RequiredCapabilities => NullCapabilities.Instance;

    protected override ActivityResult CreateFailureResult() => ActivityResult.Create(false, -1);

    protected override ActivityResult CreateResult(long resultNumber) => ActivityResult.Create(resultNumber == 0, resultNumber);
}
