// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Driver API of the RFID driver
/// </summary>
public interface IRfidDriver : IInputDriver
{
    /// <summary>
    /// Reads data from an RFID tag using the specified options.
    /// </summary>
    /// <param name="options">Options that define how the read operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    Task<RfidTag> ReadTagAsync(ReadTagOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the continuous read of RFID-tags. Raises the <see cref="TagsRead"/> when a tag was detected.
    /// </summary>
    /// <param name="options">Options that define how the read operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task StartContinuousReadAsync(ReadTagOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops automatic read of RFID-tags
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task StopContinuousReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a kill operation on the specified RFID tag, permanently erasing all information from the tag.
    /// </summary>
    /// <param name="rfidTag">The RFID tag to be killed.</param>
    /// <param name="options">Options that define how the kill operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    Task<KillingResult> KillTagAsync(RfidTag rfidTag, KillTagOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when tags are read by the antenna
    /// </summary>
    event EventHandler<RfidTag[]> TagsRead;
}
