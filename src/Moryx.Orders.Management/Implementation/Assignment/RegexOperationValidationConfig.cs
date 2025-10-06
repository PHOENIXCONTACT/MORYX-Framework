// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Orders.Assignment;

namespace Moryx.Orders.Management.Assignment
{
    /// <summary>
    /// Config for the <see cref="RegexOperationValidation"/>
    /// </summary>
    public class RegexOperationValidationConfig : OperationValidationConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(RegexOperationValidation);

        /// <summary>
        /// Creates a new instance of the <see cref="RegexOperationValidationConfig"/>
        /// </summary>
        public RegexOperationValidationConfig()
        {
            RegularExpression = @"^[\S+]{4}$";
        }

        /// <summary>
        /// Gets or sets the regular expression to validate the operation number.
        /// The validation will be started if the expression is not empty.
        /// Simple applications needn't use the regular expression.
        /// </summary>
        [DataMember]
        [DefaultValue(@"^[\S+]{4}$")]
        [Description("Regex applied to validate the operation number")]
        public string RegularExpression { get; set; }
    }
}
