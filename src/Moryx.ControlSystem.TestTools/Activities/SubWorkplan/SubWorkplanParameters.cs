// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.Workplans;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Parameter used for SubWorkplan.
    /// </summary>
    public abstract class SubWorkplanParameters : Parameters
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
        /// The SubWorkplan which should be used to go on
        /// </summary>
        public IWorkplan Workplan { get; set; }

        /// <summary>
        /// Capabilities required to run this workplan
        /// </summary>
        public ICapabilities Capabilities { get; protected set; }

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

