// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;

namespace Moryx.Communication.Sockets
{
    /// <summary>
    /// Mapping of ports and protocols to make sure we don't mix protocols on ports
    /// </summary>
    internal class PortMap
    {
        /// <summary>
        /// Application global map of ports
        /// </summary>
        private static readonly List<PortMap> Registrations = [];

        /// <summary>
        /// IP Address of the registration
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// Port of the registration
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Protocol interpreter registered at this address
        /// </summary>
        public IMessageInterpreter Interpreter { get; }

        public PortMap(IPAddress address, int port, IMessageInterpreter interpreter)
        {
            Address = address;
            Port = port;
            Interpreter = interpreter;
        }

        /// <summary>
        /// Try to register a message interpreter on a given port
        /// </summary>
        /// <param name="address">Ip address to listen on</param>
        /// <param name="port">Port to use</param>
        /// <param name="protocol">Interpreter of the protocol</param>
        /// <returns>True if registration was successful, otherwise false</returns>
        public static bool Register(IPAddress address, int port, IMessageInterpreter protocol)
        {
            lock (Registrations)
            {
                IEnumerable<PortMap> addressRelevant;
                if (IsAny(address))
                    // If this is a registration for all IPAddresses, we need to compare against all entries
                    addressRelevant = Registrations;
                else
                    // Find all registrations on the same or all addresses
                    addressRelevant = Registrations.Where(r => IsAny(r.Address) | r.Address.Equals(address));

                // Within relevant addresses find an entry with the same port
                var match = addressRelevant.FirstOrDefault(m => m.Port == port);
                if (match != null && !match.Interpreter.Equals(protocol))
                    return false; // Conflict

                // Create a registration
                Registrations.Add(new PortMap(address, port, protocol));
                return true;
            }
        }

        /// <summary>
        /// Check if a given IPAddress represents IPv4 or IPv6 any
        /// </summary>
        private static bool IsAny(IPAddress address)
        {
            return address.Equals(IPAddress.Any) | address.Equals(IPAddress.IPv6Any);
        }
    }

    /// <summary>
    /// Central server for all listener connections
    /// </summary>
    internal class TcpServer
    {
        /// <summary>
        /// Lazy ThreadSafe instance of the tcp server
        /// </summary>
        private static readonly Lazy<TcpServer> LazyInstance =
            new(() => new TcpServer(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Singleton instance of the server
        /// </summary>
        public static TcpServer Instance => LazyInstance.Value;

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
