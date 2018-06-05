using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.Communication.Sockets
{
    /// <summary>
    /// Mapping of ports and protocols to make sure we don't mix protocols on ports
    /// </summary>
    internal static class PortMap
    {
        /// <summary>
        /// Application global map of ports
        /// </summary>
        private static readonly IDictionary<int, IMessageInterpreter> Map = new Dictionary<int, IMessageInterpreter>();

        /// <summary>
        /// Try to register a message interpreter on a given port
        /// </summary>
        /// <param name="port">Port to use</param>
        /// <param name="protocol">Interpreter of the protocol</param>
        /// <returns>True if registration was successful, otherwise false</returns>
        public static bool Register(int port, IMessageInterpreter protocol)
        {
            lock (Map)
            {
                if (Map.ContainsKey(port) && !Map[port].Equals(protocol))
                    return false;

                Map[port] = protocol;
                return true;
            }
        }
    }

    /// <summary>
    /// Central server for all listener connections
    /// </summary>
    internal class TcpServer : ITcpServer
    {
        private static TcpServer _instance;
        /// <summary>
        /// Singleton instance of the server
        /// </summary>
        public static TcpServer Instance => _instance ?? (_instance = new TcpServer());

        /// <summary>
        /// List of all running listeners
        /// </summary>
        private readonly IList<TcpPortListener> _listeners = new List<TcpPortListener>();

        /// <summary>
        /// Register for a specific port
        /// </summary>
        public void Register(TcpListenerConnection listener)
        {
            // First check if port was taken
            var port = listener.Port;
            var validator = listener.Validator;
            var interpreter = validator.Interpreter;
            if (!PortMap.Register(port, interpreter))
            {
                throw new InvalidOperationException($"Attempted to register protocol header {interpreter} on port {port}, but port was already taken");
            }

            // Get port listeners
            TcpPortListener portListener;
            lock (_listeners)
            {
                portListener = _listeners.FirstOrDefault(l => l.Port == port);
                if (portListener == null)
                {
                    portListener = new TcpPortListener(port);
                    _listeners.Add(portListener);
                }
            }

            portListener.Register(listener);
        }

        /// <summary>
        /// Unregister the connection
        /// </summary>
        /// <param name="listener"></param>
        public void Unregister(TcpListenerConnection listener)
        {
            TcpPortListener target;
            lock (_listeners)
            {
                target = _listeners.First(l => l.Port == listener.Port);
            }
            target.TryUnregister(listener);
        }
    }
}