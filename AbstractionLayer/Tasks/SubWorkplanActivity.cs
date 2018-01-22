using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Activity for a subworkplan.
    /// </summary>
    public class SubWorkplanActivity : Activity<SubWorkplanParameters>
    {
        /// <processor>ClassName</processor>
        public const string TypeName = "SubWorkplanActivity";

        /// <summary>
        /// Unique type name of this instance
        /// </summary>
        public override string Type
        {
            get { return TypeName; }
        }

        ///
        public override ICapabilities RequiredCapabilities
        {
            get { return Parameters.Capilities; }
        }

        /// <summary>
        /// Specifies the special article requirements of this type
        /// </summary>
        public override ProcessRequirement ProcessRequirement
        {
            get { return Parameters.ProcessRequirements; }
        }

        /// <summary>
        /// Create a typed result object for this activity based on the result number
        /// </summary>
        /// <param name="resultNumber"></param>
        /// <returns></returns>
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
