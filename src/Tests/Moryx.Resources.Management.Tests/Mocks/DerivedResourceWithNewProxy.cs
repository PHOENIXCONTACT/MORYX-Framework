// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
{
    public interface ISecondNonResourceInterface
    {
    }

    [ResourceAvailableAs(typeof(ISecondNonResourceInterface))]
    public class DerivedResourceWithNewProxy : SimpleResource, ISecondNonResourceInterface
    {
        public override int MultiplyFoo(int factor)
        {
            return Foo *= factor + 2;
        }
    }
}
