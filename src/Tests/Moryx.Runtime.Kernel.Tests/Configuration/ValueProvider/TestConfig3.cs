// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Tests.Configuration.ValueProvider
{
    public class TestConfig3
    {
        [DataMember]
        public List<TestConfig1> Configs { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }
    }
}
