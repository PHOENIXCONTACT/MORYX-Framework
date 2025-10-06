// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Identity;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Capabilities;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Activity representing mounting activities
    /// </summary>
    [ActivityResults(typeof(MountingResult))]
    public class MountActivity : Activity<MountingParameters>, IMountingActivity, IInstanceModificationActivity
    {
        /// <inheritdoc />
        public MountOperation Operation => Result.Numeric == (int)MountingResult.Mounted
            ? MountOperation.Mount : MountOperation.Unchanged;

        /// <inheritdoc />
        public IIdentity InstanceIdentity { get; set; }

        /// <inheritdoc />
        public InstanceModificationType ModificationType { get; set; }

        /// <inheritdoc />
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.Empty;

        /// <inheritdoc />
        public override ICapabilities RequiredCapabilities => new MountCapabilities(true, false);

        /// <summary>
        /// Create a typed result object for this activity based on the result number
        /// </summary>
        protected override ActivityResult CreateResult(long resultNumber)
        {
            ModificationType = resultNumber == (int)MountingResult.Mounted ? InstanceModificationType.Created : InstanceModificationType.None;
            return ActivityResult.Create((MountingResult)resultNumber);
        }

        /// <summary>
        /// Create a typed result object for a technical failure.
        /// </summary>
        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(MountingResult.TechnicalFailure);
        }
    }
}

