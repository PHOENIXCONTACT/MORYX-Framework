// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Activities;

namespace Moryx.ControlSystem.TestTools
{
    public class DummyActivityParameters : Parameters, IActivityTimeoutParameters
    {
        public int Timeout { get; set; }

        protected override void Populate(Process process, Parameters instance)
        {
        }
    }

    public class DummyCapabilities : CapabilitiesBase
    {
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return provided is DummyCapabilities;
        }
    }

    [ActivityResults(typeof(DummyResult))]
    public class DummyActivity : Activity<DummyActivityParameters>, IInstanceModificationActivity
    {
        public IIdentity InstanceIdentity { get; set; }

        public InstanceModificationType ModificationType { get; set; }

        protected override ActivityResult CreateResult(long resultNumber)
        {
            return ActivityResult.Create((DummyResult)resultNumber);
        }

        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(DummyResult.TechnicalFailure);
        }

        public override ICapabilities RequiredCapabilities => new DummyCapabilities();

        public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;
    }

    public enum DummyResult
    {
        /// <summary>
        /// Production step was successful
        /// </summary>
        Done,

        /// <summary>
        /// Production step was not successful
        /// </summary>
        Failed,

        /// <summary>
        /// The activity could not be started at all.
        /// </summary>
        TechnicalFailure
    }
}
