// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// General non-generic interface for communication channels
    /// </summary>
    public interface IMessageChannel
    {
        /// <summary>
        /// Reference to the underlying driver of this communication
        /// </summary>
        IDriver Driver { get; }

        /// <summary>
        /// Identifier of this channel
        /// </summary>
        string Identifier { get; }
    }

    /// <summary>
    /// Interface for message based communication
    /// </summary>
    public interface IMessageChannel<in TSend, TReceive> : IMessageChannel
    {
        /// <summary>
        /// Send message through the driver
        /// </summary>
        void Send(TSend payload);

        /// <summary>
        /// Send data async through channel
        /// </summary>
        Task SendAsync(TSend payload);

        /// <summary>
        /// Event raised when the driver receives a message
        /// </summary>
        event EventHandler<TReceive> Received;
    }

    /// <summary>
    /// Interface for message based communication
    /// </summary>
    public interface IMessageChannel<TPayload> : IMessageChannel<TPayload, TPayload>
    {
    }
}
