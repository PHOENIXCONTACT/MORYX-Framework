// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Driver API of the RFID driver
/// </summary>
public interface IRfidDriver : IInputDriver,
    ISingleInput<ReadTagOptions, RfidTagReadResult>,
    IContinuousInput<ReadTagOptions, RfidTagReadResult>
{
    /// <summary>
    /// Executes a kill operation on the specified RFID tag, permanently shuts down the tag.
    /// </summary>
    /// <param name="rfidTag">The RFID tag to be killed.</param>
    /// <param name="options">Options that define how the kill operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    Task<KillTagResult> KillTagAsync(RfidTag rfidTag, KillTagOptions options, CancellationToken cancellationToken = default);
}
