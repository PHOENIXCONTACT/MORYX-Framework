// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Processes;

namespace Moryx.ControlSystem.Assemble
{
    /// <summary>
    /// General activity for assemble something. Like insert a part into a wpc
    /// </summary>
    [ActivityResults(typeof(DefaultActivityResult))]
    public abstract class AssembleActivity<TParam> : Activity<TParam, ProcessHolderTracing>, IAssembleActivity
         where TParam : AssembleParameters
    {
        /// <inheritdoc />
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

        /// <inheritdoc />
        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DefaultActivityResult)resultNumber);
        }

        /// <inheritdoc />
        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DefaultActivityResult.TechnicalError);
        }

        AssembleParameters IActivity<AssembleParameters>.Parameters => Parameters;
    }

    /// <summary>
    /// General activity for assemble something. Like insert a part into a wpc
    /// </summary>
    public abstract class AssembleActivity : AssembleActivity<AssembleParameters>
    {
    }
}
