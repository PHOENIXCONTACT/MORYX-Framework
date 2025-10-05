// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Drivers.Message;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Message object used to directly read and write nodes using the Opc Ua Driver
/// </summary>
public class OpcUaMessage : IIdentifierMessage
{
    /// <summary>
    /// Opc Ua Node Id
    /// </summary>
    public string Identifier { get; set; }

    /// <summary>
    /// Value to be written on the Opc Ua Node
    /// </summary>
    public object Payload { get; set; }
}
