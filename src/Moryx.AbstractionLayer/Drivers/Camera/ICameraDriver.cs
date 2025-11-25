// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Camera;

/// <summary>
/// Interface for camera devices, that provide image data
/// </summary>
public interface ICameraDriver<TImage> : IInputDriver where TImage : class
{
    /// <summary>
    /// Capture a single image from the camera
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns>The image that was captured</returns>
    /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<TImage> CaptureImageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event to continuously provide images from a camera
    /// </summary>
    event EventHandler<TImage> CapturedImage;
}
