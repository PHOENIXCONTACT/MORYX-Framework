// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Runtime.Kernel.Tests.Dummies
{
    /// <summary>
    /// Testenum
    /// </summary>
    public enum TestConfig1Enum
    {
        /// <summary>
        /// The enum value1
        /// </summary>
        EnumValue1,
        /// <summary>
        /// The enum value2
        /// </summary>
        EnumValue2,
        /// <summary>
        /// The enum value3
        /// </summary>
        EnumValue3
    }

    /// <summary>
    /// Test configuration
    /// </summary>
    [DataContract]
    public class RuntimeConfigManagerTestConfig1 : ConfigBase, IUpdatableConfig
    {
        /// <summary>
        /// The boolean field default
        /// </summary>
        public const bool BooleanFieldDefault = true;
        /// <summary>
        /// The string field default
        /// </summary>
        public const string StringFieldDefault = "42";
        /// <summary>
        /// The int field default
        /// </summary>
        public const int IntFieldDefault = 43;
        /// <summary>
        /// The double field default
        /// </summary>
        public const double DoubleFieldDefault = 44.1;
        /// <summary>
        /// The long field default
        /// </summary>
        public const long LongFieldDefault = 45;
        /// <summary>
        /// The byte field default
        /// </summary>
        public const byte ByteFieldDefault = 255;
        /// <summary>
        /// The enum field default
        /// </summary>
        public const TestConfig1Enum EnumFieldDefault = TestConfig1Enum.EnumValue3;

        /// <summary>
        /// Gets or sets a nullable boolean value.
        /// </summary>
        [DataMember]
        [DefaultValue(BooleanFieldDefault)]
        public bool? NullableBooleanField { get; set; }

        /// <summary>
        /// Gets or sets a boolean value.
        /// </summary>
        [DataMember]
        [DefaultValue(BooleanFieldDefault)]
        public bool BooleanField { get; set; }

        /// <summary>
        /// Gets or sets a string value.
        /// </summary>
        [DataMember]
        [DefaultValue(StringFieldDefault)]
        public string StringField { get; set; }

        /// <summary>
        /// Gets or sets a int value.
        /// </summary>
        [DataMember]
        [DefaultValue(IntFieldDefault)]
        public int IntField { get; set; }

        /// <summary>
        /// Gets or sets a double value.
        /// </summary>
        [DataMember]
        [DefaultValue(DoubleFieldDefault)]
        public double DoubleField { get; set; }

        /// <summary>
        /// Gets or sets a sub configuration value.
        /// </summary>
        [DataMember]
        public RuntimeConfigManagerTestConfig2 SubConfig { get; set; }

        /// <summary>
        /// Gets or sets a long value.
        /// </summary>
        [DataMember]
        [DefaultValue(LongFieldDefault)]
        public long LongField { get; set; }

        /// <summary>
        /// Gets or sets a byte value.
        /// </summary>
        [DataMember]
        [DefaultValue(ByteFieldDefault)]
        public byte ByteField { get; set; }

        /// <summary>
        /// Gets or sets a enum value.
        /// </summary>
        [DataMember]
        [DefaultValue(EnumFieldDefault)]
        public TestConfig1Enum EnumField { get; set; }

        /// <summary>
        /// External raise method invoked by <see cref="T:Moryx.Configuration.IConfigManager" />
        /// </summary>
        /// <param name="modifiedProperties">Properties modified</param>
        public void RaiseConfigChanged(params string[] modifiedProperties)
        {
            if (ConfigChanged != null)
                ConfigChanged(this, new ConfigChangedEventArgs(modifiedProperties));
        }

        /// <summary>
        /// Event raised when the config was modified
        /// </summary>
        public event EventHandler<ConfigChangedEventArgs> ConfigChanged;
    }
}
