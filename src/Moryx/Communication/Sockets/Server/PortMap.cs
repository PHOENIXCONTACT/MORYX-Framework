// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;

namespace Moryx.Communication.Sockets;

/// <summary>
/// Mapping of ports and protocols to make sure we don't mix protocols on ports
/// </summary>
internal class PortMap
{
    /// <summary>
    /// Application global map of ports
    /// </summary>
    private static readonly List<PortMap> _registrations = [];

    /// <summary>
    /// IP Address of the registration
    /// </summary>
    private readonly IPAddress _address;

    /// <summary>
    /// Port of the registration
    /// </summary>
    private readonly int _port;

    /// <summary>
    /// Protocol interpreter registered at this address
    /// </summary>
    private readonly IMessageInterpreter _interpreter;

    public PortMap(IPAddress address, int port, IMessageInterpreter interpreter)
    {
        _address = address;
        _port = port;
        _interpreter = interpreter;
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
        lock (_registrations)
        {
            IEnumerable<PortMap> addressRelevant;
            if (IsAny(address))
                // If this is a registration for all IPAddresses, we need to compare against all entries
                addressRelevant = _registrations;
            else
                // Find all registrations on the same or all addresses
                addressRelevant = _registrations.Where(r => IsAny(r._address) | r._address.Equals(address));

            // Within relevant addresses find an entry with the same port
            var match = addressRelevant.FirstOrDefault(m => m._port == port);
            if (match != null && !match._interpreter.Equals(protocol))
                return false; // Conflict

            // Create a registration
            _registrations.Add(new PortMap(address, port, protocol));
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
