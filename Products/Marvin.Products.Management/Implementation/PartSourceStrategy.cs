namespace Marvin.Products.Management
{
    /// <summary>
    /// Different types of article creation
    /// </summary>
    public enum PartSourceStrategy
    {
        /// <summary>
        /// Default article creation from partlink
        /// </summary>
        FromPartlink = 0,

        /// <summary>
        /// Create parts from entity relation
        /// </summary>
        FromEntities = 10,
    }
}