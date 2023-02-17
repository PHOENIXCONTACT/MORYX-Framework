// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.TestModule
{
    /// <summary>
    /// Public API for TestModule
    /// </summary
    public interface ITestModule
    {
        int Bla { get; }

        int Foo(int a, int b);
    }
}
