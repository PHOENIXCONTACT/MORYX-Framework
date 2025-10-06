// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.ControlSystem.Capabilities;
using Moryx.ControlSystem.Setups;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Setup trigger config for assemble cells
    /// </summary>
    public class AssembleSetupTriggerConfig : SetupTriggerConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(AssembleSetupTrigger);

        /// <summary>
        /// Trigger is executed before or after production job
        /// </summary>
        [DataMember, Description("Trigger is executed before or after production job")]
        public SetupExecution Execution { get; set; }

        /// <summary>
        /// Configures the classification of this trigger
        /// </summary>
        [DataMember, Description("Classification if the trigger")]
        public SetupClassification SetupClassification { get; set; } = SetupClassification.Unspecified;

        /// <summary>
        /// Required capability type to find the resource which should be setup
        /// </summary>
        [DataMember, Description("Required capability type to find the resource which should be setup")]
        [PossibleTypes(typeof(AssembleCapabilities))]
        public string RequiredCapabilityType { get; set; }

        /// <summary>
        /// Pairs of binding condition and instruction text.
        /// </summary>
        [DataMember, Description("Bindable visual instruction text")]
        public string Instruction { get; set; }

        /// <summary>
        /// Binding string that describes value source in recipe
        /// </summary>
        [DataMember, Description("Binding string that describes value source in recipe.")]
        public string ValueSource { get; set; }

        /// <summary>
        /// Name of property within capabilities
        /// </summary>
        [DataMember, Description("Name of property within capability.")]
        public string TargetPropertyName { get; set; }
    }
}

