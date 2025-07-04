// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.VisualInstructions;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Setup parameters for assemble cell
    /// </summary>
    public class AssembleSetupParameters : VisualInstructionParameters
    {
        /// <summary>
        /// The capabilities the assemble cell should have after the setup
        /// </summary>
        public ICapabilities TargetCapabilities { get; set; }

        /// <summary>
        /// The capabilities which an assemble cell should currently have to find it for a setup
        /// </summary>
        public ICapabilities RequiredCapabilities { get; set; }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            base.Populate(process, instance);
            var parameters = (AssembleSetupParameters)instance;
            parameters.TargetCapabilities = TargetCapabilities;
            parameters.RequiredCapabilities = RequiredCapabilities;
        }
    }

    /// <summary>
    /// Setup parameters for assemble cell which needs descriptors for a setup
    /// </summary>
    public class AssembleDescriptorSetupParameters : AssembleSetupParameters
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Value of the property
        /// </summary>
        public object Value { get; set; }

        /// <inheritdoc />
        protected override void Populate(IProcess process, Parameters instance)
        {
            base.Populate(process, instance);
            var parameters = (AssembleDescriptorSetupParameters)instance;
            parameters.PropertyName = PropertyName;
            parameters.Value = Value;
        }
    }
}

