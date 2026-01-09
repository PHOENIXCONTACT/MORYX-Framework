// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis;

/// <summary>
/// Driver that can control axes
/// </summary>
public interface IAxesController : IDriver
{
    /// <summary>
    /// Will move the axes of the system to the given position
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <param name="movement">Array of axes which should be moved</param>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="MoveAxesException">Will be thrown for errors during moving axes</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<AxisMovementResponse> MoveAxesAsync(CancellationToken cancellationToken = default, params AxisMovement[] movement);
}