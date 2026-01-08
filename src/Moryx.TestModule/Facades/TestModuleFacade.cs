// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.TestModule;

public class TestModuleFacade : FacadeBase, ITestModule
{
    public int Bla => ++field;

    public int Foo(int a, int b) => a + b;
}