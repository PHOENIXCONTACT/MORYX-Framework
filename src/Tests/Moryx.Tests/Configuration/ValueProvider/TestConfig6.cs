// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tests.Configuration.ValueProvider
{
    internal class TestConfig6
    {
        public TestConfig6()
        {
        }

        [DefaultValue(5)]
        public int? WithDefaultValue { get; set; }

        [DefaultValue(null)]
        public int? WithDefaultValueNull { get; set; }

        public int? WithoutDefaultValue { get; set; }
    }
}