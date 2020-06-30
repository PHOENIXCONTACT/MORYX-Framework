// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class TestConfig1 : ConfigBase
    {
        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumberReadOnly { get; } = 1024;

        [DataMember]
        public int DummyNumber2 { get; } = 25;

        [DataMember]
        [DefaultValue(DefaultValues.Text)]
        public string DummyText { get; set; }
    }
}
