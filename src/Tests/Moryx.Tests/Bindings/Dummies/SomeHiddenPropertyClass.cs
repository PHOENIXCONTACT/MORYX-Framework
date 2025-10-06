// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Bindings
{
    public class SomeHiddenPropertyClass : SomeClass
    {
        public new SomeImplementation SomeObject
        {
            get { return (SomeImplementation)base.SomeObject; }
            set { base.SomeObject = value; }
        }
    }
}
