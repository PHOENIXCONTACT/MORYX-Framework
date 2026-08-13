// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.TestTools.Resources;

public class DerivedResource : SimpleResource
{
    public override int MultiplyFoo(int factor)
    {
        return Foo *= factor + 1;
    }
}
