// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message;

/// <summary>
/// Generic implementation of a <see cref="IMessageChannel"/> which can filter the messages of <see cref="IMessageDriver"/> by a given func.
/// </summary>
public sealed class MessageFilterChannel : IMessageChannel, IDisposable
{
    private readonly Func<object, bool> _messageFilter;
    private readonly IMessageDriver _underlyingDriver;

    /// <inheritdoc />
    public IDriver Driver => _underlyingDriver;

    /// <inheritdoc />
    public string Identifier { get; }

    /// <summary>
    /// Creates a new instance of <see cref="MessageFilterChannel"/>
    /// </summary>
    /// <param name="underlyingDriver">Underlying driver for the filter.</param>
    /// <param name="identifier">Identifier of this channel</param>
    /// <param name="messageFilter">Filter function for the messages.</param>
    public MessageFilterChannel(IMessageDriver underlyingDriver, string identifier, Func<object, bool> messageFilter)
    {
        Identifier = identifier;

        _messageFilter = messageFilter;
        _underlyingDriver = underlyingDriver;
        _underlyingDriver.Received += OnUnderlyingDriverMessageReceived;
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        _underlyingDriver.Received -= OnUnderlyingDriverMessageReceived;
    }

    private void OnUnderlyingDriverMessageReceived(object sender, object e)
    {
        //Apply filter
        if (!_messageFilter(e))
        {
            return;
        }

        // Raise event
        Received?.Invoke(sender, e);
    }

    /// <inheritdoc />
    public void Send(object payload)
    {
        _underlyingDriver.Send(payload);
    }

    /// <inheritdoc />
    public Task SendAsync(object payload, CancellationToken cancellationToken = default)
    {
        return _underlyingDriver.SendAsync(payload, cancellationToken);
    }

    /// <inheritdoc />
    public event EventHandler<object> Received;
}
