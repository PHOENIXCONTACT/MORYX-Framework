namespace Marvin.Workflows
{
    /// <summary>
    /// Strategy to resolve array index for a certain mapping value
    /// </summary>
    public interface IIndexResolver
    {
        /// <summary>
        /// Resolve index by mapping value
        /// </summary>
        int Resolve(long mappingValue);
    }
}