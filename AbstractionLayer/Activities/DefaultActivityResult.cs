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
        Success = 0,

        /// <summary>
        /// The activity was failed
        /// </summary>
        Failed = 1,

        /// <summary>
        /// The activity was failed because of an technical error
        /// </summary>
        TechnicalError = 2
    }
}
