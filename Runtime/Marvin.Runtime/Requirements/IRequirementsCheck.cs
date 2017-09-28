namespace Marvin.Runtime.Requirements
{
    /// <summary>
    /// API of a requirements check plugin
    /// </summary>
    public interface IRequirementsCheck
    {
        /// <summary>
        /// Check whether a runtime requirement is fullfilled.
        /// </summary>
        /// <exception cref="System.Exception">Throws some kind of exception if the check is not passed to kill the HOG process</exception>
        void Check();
    }
}