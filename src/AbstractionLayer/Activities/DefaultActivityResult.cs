using Marvin.Workflows;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Default results for activities. 
    /// </summary>
    public enum DefaultActivityResult
    {
        /// <summary>
        /// Activity was successfull
        /// </summary>
        [OutputType(OutputType.Success)]
        Success = 0,

        /// <summary>
        /// The activity was failed
        /// </summary>
        [OutputType(OutputType.Failure)]
        Failed = 1,

        /// <summary>
        /// The activity was failed because of an technical error
        /// </summary>
        [OutputType(OutputType.Failure)]
        TechnicalError = 2
    }
}
