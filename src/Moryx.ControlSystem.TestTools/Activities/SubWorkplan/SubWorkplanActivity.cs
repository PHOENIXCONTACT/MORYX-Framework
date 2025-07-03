using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Activity for a subworkplan.
    /// </summary>
    public class SubWorkplanActivity : Activity<SubWorkplanParameters>
    {
        ///
        public override ICapabilities RequiredCapabilities => Parameters.Capabilities;

        /// <summary>
        /// Specifies the special process requirements of this type
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
