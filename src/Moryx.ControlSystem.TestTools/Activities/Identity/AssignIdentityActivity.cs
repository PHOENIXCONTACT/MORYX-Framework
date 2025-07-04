using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Identity;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Capabilities;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Assign an identity to the given target
    /// </summary>
    [ActivityResults(typeof(AssignIdentityResults))]
    public class AssignIdentityActivity : Activity<IAssignIdentityParameters>, IInstanceModificationActivity
    {
        /// <inheritdoc />
        public IIdentity InstanceIdentity { get; set; }

        /// <inheritdoc />
        public InstanceModificationType ModificationType { get; set; }

        /// <inheritdoc />
        public override ProcessRequirement ProcessRequirement => ProcessRequirement.NotRequired;

        /// <inheritdoc />
        public override ICapabilities RequiredCapabilities => new AssignIdentityCapabilities(Parameters.Source, Parameters.Type);

        /// <summary>
        /// Create a typed result object for this result number
        /// </summary>
        protected override ActivityResult CreateResult(long resultNumber)
        {
            ModificationType = resultNumber == (int) AssignIdentityResults.Assigned
                ? InstanceModificationType.Changed
                : InstanceModificationType.None;

            return ActivityResult.Create((AssignIdentityResults)resultNumber);
        }

        /// <summary>
        /// Create a typed result object for a technical failure.
        /// </summary>
        protected override ActivityResult CreateFailureResult()
        {
            return ActivityResult.Create(AssignIdentityResults.TechnicalFailure);
        }
    }
}
