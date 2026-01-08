// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Central server for all listener connections
    /// </summary>
    internal class TcpServer
    {
        /// <summary>
        /// Lazy ThreadSafe instance of the tcp server
        /// </summary>
        private static readonly Lazy<TcpServer> _lazyInstance =
            new(() => new TcpServer(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Singleton instance of the server
        /// </summary>
        public static TcpServer Instance => _lazyInstance.Value;

        /// <summary>
        /// List of all running listeners
        /// </summary>
        private readonly List<TcpPortListener> _listeners = [];

        /// <summary>
        /// Register for a specific port
        /// </summary>
        public void Register(TcpListenerConnection listener)
        {
            // First check if port was taken
            var port = listener.Port;
            var address = listener.Address;
            var interpreter = listener.Validator.Interpreter;
            if (!PortMap.Register(address, port, interpreter))
            {
                throw new InvalidOperationException($"Attempted to register protocol header {interpreter} on port {port}, but port was already taken");
            }

            // Get port listeners
            TcpPortListener portListener;
            lock (_listeners)
            {
                portListener = FindListener(listener);
                if (portListener == null)
                {
                    portListener = new TcpPortListener(address, port);
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
                target = FindListener(listener);
            }
            target.TryUnregister(listener);
        }

        private TcpPortListener FindListener(TcpListenerConnection listener)
        {
            return _listeners.FirstOrDefault(l => l.Address.Equals(listener.Address) && l.Port == listener.Port);
        }
    }
}
