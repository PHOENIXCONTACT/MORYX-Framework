// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Runtime.Kernel.Tests.Configuration
{
    /// <summary>
    /// Default values that must be set by ConfigManager
    /// </summary>
    public static class DefaultValues
    {
        public const int Number = 42;

        public const double Decimal = 3.1415;

        public const string Text = "Hello";
    }

    public static class ModifiedValues
    {
        public const int Number = 84;

        public const double Decimal = 2.719;

        public const string Text = "Hello Thomas";
    }

    [DataContract]
    public class TestConfig : ConfigBase
    {
        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public int DummyNumber { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Number)]
        public ushort DummyShort { get; set; }

        [DataMember]
        [DefaultValue(DefaultValues.Text)]
        public string DummyString { get; set; }

        [DataMember]
        public ChildConfig Child { get; set; }
    }

    public class ChildConfig : UpdatableEntry
    {
        [DataMember]
        [DefaultValue(DefaultValues.Decimal)]
        public double DummyDouble { get; set; }
    }

    public class NonPersistedTestConfig : TestConfig
    {
    }

    [DataContract]
    public class SecretConfig : ConfigBase
    {
        [DataMember]
        public string NormalSetting { get; set; }

        [DataMember]
        [Password]
        public string ServiceApiKey { get; set; }
    }
}
