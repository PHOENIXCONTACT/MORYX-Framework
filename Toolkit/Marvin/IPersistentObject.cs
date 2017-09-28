namespace Marvin
{
    /// <summary>
    /// Common interface for all business objects containing a database ID
    /// </summary>
    public interface IPersistentObject
    {
        /// <summary>
        /// Id of this object
        /// </summary>
        long Id { get; set; }
    }
}