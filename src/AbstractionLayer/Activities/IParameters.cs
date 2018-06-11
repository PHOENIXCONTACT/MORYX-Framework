namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Common interface for all parameters
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Create new parameters object with resolved binding values from process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        IParameters Bind(IProcess process);
    }
}