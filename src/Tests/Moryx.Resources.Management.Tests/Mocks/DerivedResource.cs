// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Resources.Management.Tests
{
    public class DerivedResource : SimpleResource
    {
        public override int MultiplyFoo(int factor)
        {
            return Foo *= factor + 1;
        }
    }
}
