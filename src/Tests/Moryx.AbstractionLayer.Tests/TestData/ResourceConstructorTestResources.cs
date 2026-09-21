// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.AbstractionLayer.Tests.TestData;

internal class GeneralConstructorResource : Resource
{
    [ResourceConstructor]
    public virtual void ConstructGeneralResource() => throw new NotImplementedException();
}

internal class SpecificConstructorResource : GeneralConstructorResource
{
    public override void ConstructGeneralResource() => throw new NotImplementedException();

    [ResourceConstructor]
    public void ConstructSpecificResource() => throw new NotImplementedException();
}
