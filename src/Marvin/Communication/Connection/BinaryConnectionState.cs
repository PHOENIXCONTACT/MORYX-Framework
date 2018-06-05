namespace Marvin.Communication
{
    /// <summary>
    /// Possible states of a binary connection  
    /// </summary>
    public enum BinaryConnectionState
    {
        /// <summary>
        /// No connection available
        /// </summary>
        Disconnected,

        /// <summary>
        /// Component is trying to establish a connection
        /// </summary>
        AttemptingConnection,

        /// <summary>
        /// Component is connected. Communication is possible
        /// </summary>
        Connected
    }
}
