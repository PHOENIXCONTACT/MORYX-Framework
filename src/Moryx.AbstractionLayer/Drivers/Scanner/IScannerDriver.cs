// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Scanner;

/// <summary>
/// Common interface for barcode / QR-Code scanners
/// </summary>
public interface IScannerDriver : IInputDriver
{
    /// <summary>
    /// Read a single code
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<ScannerResult> SingleReadAsync(ReadCodeOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the continuous read of codes. Raises the <see cref="CodeRead"/> when a code was detected.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task StartContinuousReadAsync(ReadCodeOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops automatic read of codes
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task StopContinuousReadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when a code was read
    /// </summary>
    event EventHandler<ScannerResult> CodeRead;
}
