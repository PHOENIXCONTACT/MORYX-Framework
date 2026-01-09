// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.InOut;

/// <summary>
/// Interface for drivers providing single inputs
/// </summary>
/// <typeparam name="TResult">Type which is used for the result</typeparam>
/// <typeparam name="TOptions">Options used for automatic read of values</typeparam>
public interface ISingleInput<in TOptions, TResult>
{
    /// <summary>
    /// Reads a single value from the device
    /// </summary>
    /// <param name="options">Options that define how the operation should be performed.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<TResult> ReadAsync(TOptions options, CancellationToken cancellationToken = default);
}
