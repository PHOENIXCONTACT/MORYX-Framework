// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Camera
{
    /// <summary>
    /// Interface for camera devices, that provide image data
    /// </summary>
    public interface ICameraDriver<TImage> : IDriver where TImage : class
    {
        /// <summary>
        /// Eventhandler to continously provide images from a camera
        /// </summary>
        event EventHandler<TImage> CapturedImage;

        /// <summary>
        /// Capture a single image from the camera
        /// </summary>
        /// <returns>
        ///     The image that was captured or null in case no image
        ///     could be retrieved
        /// </returns>
        Task<TImage> CaptureImageAsync();
    }
}

