// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Scales;

/// <summary>
/// Interface for weight scales to request a weight
/// </summary>
public interface IWeightScaleDriver : IInputDriver,
    ISingleInput<MeasureOptions, MeasuredWeight>,
    IContinuousInput<MeasureOptions, MeasuredWeight>
{
    /// <summary>
    /// Performs a tare operation on the weight scale, resetting its measurement to zero.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <exception cref="DriverException">Thrown when the driver encounters an error during execution.</exception>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task TaraAsync(CancellationToken cancellationToken = default);
}
