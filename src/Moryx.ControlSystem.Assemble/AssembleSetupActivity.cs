// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Activities;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// Setup activity for an AssembleCell
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    [ActivityResults(typeof(DefaultActivityResult))]
    public abstract class AssembleSetupActivity<TParam> : Activity<TParam>, IAssembleSetupActivity
        where TParam : AssembleSetupParameters
    {
        /// <inheritdoc />
        public sealed override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        /// <inheritdoc />
        public ActivityClassification Classification => ActivityClassification.Setup;

        /// <summary>
        /// Creates a failure result
        /// </summary>
        /// <returns>Always an <see cref="ActivityResult"/> with <see cref="DefaultActivityResult.TechnicalError"/></returns>
        protected sealed override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DefaultActivityResult.TechnicalError);
        }

        /// <inheritdoc />
        protected sealed override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DefaultActivityResult)resultNumber);
        }
    }


    /// <summary>
    /// Setup activity for an AssembleCell
    /// </summary>
    public class AssembleSetupActivity : AssembleSetupActivity<AssembleDescriptorSetupParameters>
    {
        /// <inheritdoc />
        public override ICapabilities RequiredCapabilities => Parameters.RequiredCapabilities;
    }
}

