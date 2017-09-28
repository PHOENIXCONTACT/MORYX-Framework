namespace Marvin.Users
{
    /// <summary>
    /// Access rights of the current user for a certain operation
    /// </summary>
    public enum OperationAccess
    {
        /// <summary>
        /// Access is denied. Views will be hidden and buttons removed
        /// </summary>
        Denied = 0,
        /// <summary>
        /// Only certain data is visible
        /// </summary>
        LimitedRead = 1,
        /// <summary>
        /// All data can be read, but not modified
        /// </summary>
        ReadOnly = 2,
        /// <summary>
        /// All data can be accessed and modified
        /// </summary>
        Full = 3,
    }
}
