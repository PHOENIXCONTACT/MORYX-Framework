// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Multipurpose driver that exchanges information with a device
    /// </summary>
    public interface IMessageDriver : IDriver
    {
        /// <summary>
        /// Send data async through channel
        /// </summary>
        /// <param name="payload">Message to send through the driver</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
        Task SendAsync(object payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// Event raised when the driver receives a message
        /// </summary>
        event EventHandler<object> Received;
    }
}
