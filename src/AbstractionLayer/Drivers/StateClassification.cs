namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Classification of states from application point of view
    /// </summary>
    public enum StateClassification
    {
        /// <summary>
        /// Offline means not reacheable
        /// </summary>
        Offline,

        /// <summary>
        /// Initializing means preparing or starting
        /// </summary>
        Initializing,

        /// <summary>
        /// Running means ready to work or working
        /// </summary>
        Running,

        /// <summary>
        /// Busy means that the driver is running but is allready in work
        /// </summary>
        Busy,

        /// <summary>
        /// Maintenance means waiting for maintenance or maintenance running
        /// </summary>
        Maintenance,

        /// <summary>
        /// Error means that is not running because there is an error
        /// </summary>
        Error,
    }
}