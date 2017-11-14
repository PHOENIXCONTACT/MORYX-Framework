namespace Marvin.Communication.Sockets
{
    /// <summary>
    /// Interface for the internal TCP server
    /// </summary>
    internal interface ITcpServer
    {
        /// <summary>
        /// Register for a specific port
        /// </summary>
        void Register(TcpListenerConnection listener);

        /// <summary>
        /// Unregister the connection
        /// </summary>
        /// <param name="listener"></param>
        void Unregister(TcpListenerConnection listener);
    }
}