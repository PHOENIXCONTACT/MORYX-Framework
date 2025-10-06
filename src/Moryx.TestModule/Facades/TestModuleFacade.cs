// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.TestModule
{
    public class TestModuleFacade : FacadeBase, ITestModule
    {
        private int _bla;

        public int Bla => ++_bla;

        public int Foo(int a, int b) => a + b;
    }
}
