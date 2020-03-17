// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Capabilities;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Activity for a subworkplan.
    /// </summary>
    public class SubWorkplanActivity : Activity<SubWorkplanParameters>
    {
        ///
        public override ICapabilities RequiredCapabilities => Parameters.Capilities;

        /// <summary>
        /// Specifies the special article requirements of this type
        /// </summary>
        public override ProcessRequirement ProcessRequirement => Parameters.ProcessRequirements;

        /// <summary>
        /// Create a typed result object for this activity based on the result number
        /// </summary>
        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create(resultNumber == Parameters.SuccessResult, resultNumber);
        }

        /// <summary>
        /// Create a typed result object for a technical failure.
        /// </summary>
        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(false, Parameters.FailureResult);
        }
    }
}
