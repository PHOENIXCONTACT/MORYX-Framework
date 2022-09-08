// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Tests.Configuration.ValueProvider
{
    public class TestConfig2
    {
        [DataMember]
        public TestConfig1 Config { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }
    }
}
