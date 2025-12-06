// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;

namespace Moryx.Runtime.Kernel.Tests.Configuration.ValueProvider
{
    internal class ClassWithoutParamLessCtor
    {
        public ClassWithoutParamLessCtor(string a)
        {

        }
    }

    internal class TestConfig5
    {
        [DefaultValue(null)]
        public ClassWithoutParamLessCtor Field1 { get; set; }

        [DefaultValue(null)]
        public Func<string, bool> Field2 { get; set; }
    }
}