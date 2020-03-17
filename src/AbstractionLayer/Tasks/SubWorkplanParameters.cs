// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Capabilities;
using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Parameter used for SubWorkplan.
    /// </summary>
    public abstract class SubWorkplanParameters : ParametersBase
    {
        /// <summary>
        /// Default constructor for fresh object
        /// </summary>
        protected SubWorkplanParameters()
        {
        }

        /// <summary>
        /// Create parameters from current parameters with binding
        /// </summary>
        protected SubWorkplanParameters(SubWorkplanParameters original)
        {
            Workplan = original.Workplan;
            SuccessResult = original.SuccessResult;
            FailureResult = original.FailureResult;
        }

        /// <summary>
        /// The subworkplan which should be used to go on.
        /// </summary>
        public IWorkplan Workplan { get; set; }

        /// <summary>
        /// Capabilities required to run this workplan
        /// </summary>
        public ICapabilities Capilities { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessRequirement ProcessRequirements { get; protected set; }

        // ------------------>
        // TODO: There must be a better way
        internal long FailureResult { get; set; }
        internal long SuccessResult { get; set; }
        // ------------------>
    }
}
