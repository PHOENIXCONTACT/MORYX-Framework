// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Runtime.Kernel.Tests.Configuration.Transformer
{
    /// <summary>
    /// Testconfig enum
    /// </summary>
    internal enum TestConfig1Enum
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
    /// Test config
    /// </summary>
    internal class TransformerTestConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformerTestConfig"/> class.
        /// </summary>
        public TransformerTestConfig()
        {
            SubConfig = new SubConfig();
            SubConfigList = [];
        }

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
        /// The enum field default
        /// </summary>
        public const TestConfig1Enum EnumFieldDefault = TestConfig1Enum.EnumValue3;

        /// <summary>
        /// String field display name
        /// </summary>
        public const string StringFieldDisplayName = "My string Field";

        // no attributes: for some testcases
        /// <summary>
        /// Gets or sets a boolean value.
        /// </summary>
        public bool BooleanField { get; set; }

        /// <summary>
        /// Gets or sets a string value.
        /// </summary>
        [DefaultValue(StringFieldDefault)]
        [PrimitiveValues("String1", "String2", "String3")]
        [Description("StringField Test description")]
        [DisplayName(StringFieldDisplayName)]
        [Required, MinLength(3), MaxLength(10), Password]
        public string StringField { get; set; }

        /// <summary>
        /// Gets the read only property.
        /// </summary>
        public string ReadOnlyProperty => "Test";

        /// <summary>
        /// Gets or sets a int value.
        /// </summary>
        [DefaultValue(IntFieldDefault)]
        [IntegerSteps(40, 45, 3, StepMode.Addition)]
        [Description("IntField Test description")]
        public int IntField { get; set; }

        /// <summary>
        /// Gets or sets a double value.
        /// </summary>
        [DefaultValue(DoubleFieldDefault)]
        [PrimitiveValues(0.0, 0.1, 0.2)]
        [Description("DoubleField Test description")]
        public double DoubleField { get; set; }

        /// <summary>
        /// Gets or sets a long value.
        /// </summary>
        [DefaultValue(LongFieldDefault)]
        [IntegerSteps(1, 10, 3, StepMode.Multiplication)]
        [Description("LongField Test description")]
        public long LongField { get; set; }

        /// <summary>
        /// Gets or sets a enum value.
        /// </summary>
        [DefaultValue(EnumFieldDefault)]
        [Description("EnumField Test description")]
        public TestConfig1Enum EnumField { get; set; }

        /// <summary>
        /// Gets or sets a list of subconfigs.
        /// </summary>
        [Description("SubConfigList Test description")]
        public List<SubConfig> SubConfigList { get; set; }

        /// <summary>
        /// Gets or sets a subconfig.
        /// </summary>
        [Description("SubConfig Test description")]
        public SubConfig SubConfig { get; set; }
    }

    /// <summary>
    /// Test subconfig
    /// </summary>
    internal class SubConfig
    {
        /// <summary>
        /// Gets or sets the int field.
        /// </summary>
        [Description("IntField Test description")]
        public int IntField { get; set; }
    }
}
