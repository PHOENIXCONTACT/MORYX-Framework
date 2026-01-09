// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut;

/// <summary>
/// Interface for drivers providing continuous inputs
/// </summary>
/// <typeparam name="TResult">Type which is used for the result</typeparam>
/// <typeparam name="TOptions">Options used for automatic read of values</typeparam>
public interface IContinuousInput<in TOptions, TResult>
{
    /// <summary>
    /// Flag if the device is activated by <see cref="ActivateAsync"/> and receives values automatically
    /// </summary>
    bool IsActivated { get; }

    /// <summary>
    /// Activates the device to read values automatically. Raises the <see cref="ValueRead"/> when a value was read.
    /// </summary>
    /// <param name="options">Options that define how the operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task ActivateAsync(TOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates the device. The event <see cref="cancellationToken"/> is not raised for new values anymore.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="OperationCanceledException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="ValueRead">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task DeactivateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs when a value was received.
    /// </summary>
    event EventHandler<TResult> ValueRead;
}
