// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.Drivers.OpcUa;

/// <summary>
/// Class to read OpcUaNodes as Outputs
/// </summary>
public class OpcUaOutput(OpcUaDriver driver) : IOutput<object>
{
    private readonly OpcUaDriver _driver = driver;

    /// <inheritdoc/>
    public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// Gets or sets the value of a variable OpcUaNode
    /// </summary>
    /// <param name="key">Full NodeId</param>
    /// <returns></returns>
    public object this[string key] { get => GetValue(key); set => SetValue(key, value); }

    private void SetValue(string key, object value)
    {
        var node = _driver.GetNode(key);
        if (node == null)
        {
            _driver.Logger.Log(LogLevel.Warning, "Node {key} is not known. So the value wasn't written.", key);
            return;
        }
        _driver.WriteNode(node, value);
    }

    private object GetValue(string key)
    {
        return _driver.ReadNode(key);
    }

    /// <inheritdoc/>
    public SupportedAccess Access => SupportedAccess.Key;

    /// <inheritdoc/>
    public object Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
