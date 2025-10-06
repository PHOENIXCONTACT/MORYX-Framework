// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;
using Moryx.AbstractionLayer.Drivers.Message;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Class to read OpcUaNodes as Inputs
/// </summary>
public class OpcUaInput : IInput<object>
{
    public OpcUaInput(OpcUaDriver driver)
    {
        _driver = driver;
        ((IMessageDriver<OpcUaMessage>)_driver).Received += OnReceived;
    }

    private void OnReceived(object sender, OpcUaMessage e)
    {
        var eventArgs = new InputChangedEventArgs(e.Identifier);
        InputChanged?.Invoke(this, eventArgs);
    }

    private readonly OpcUaDriver _driver;

    /// <inheritdoc/>
    public object this[int index] => throw new NotImplementedException();

    /// <summary>
    /// Get the value of a variable node via the NodeId
    /// </summary>
    /// <param name="key">Full NodeId</param>
    /// <returns>If node doesn't exists or there was an error, when trying to read
    /// the node, the return value will be null</returns>
    public object this[string key] => GetValue(key);

    private object GetValue(string key)
    {
        return _driver.ReadNode(key);
    }

    /// <inheritdoc/>
    public SupportedAccess Access => SupportedAccess.Key | SupportedAccess.Event;

    /// <inheritdoc/>
    public object Value => throw new NotImplementedException();

    /// <inheritdoc/>
    public event EventHandler<InputChangedEventArgs> InputChanged;
}
