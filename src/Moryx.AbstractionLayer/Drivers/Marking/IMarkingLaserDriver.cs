// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Driver interface for laser printing devices
    /// </summary>
    public interface IMarkingLaserDriver : IDriver
    {
        /// <summary>
        /// Set up marking file as a preparation for the marking process
        /// </summary>
        /// <param name="file">The marking file used for the marking system</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        /// <exception cref="MarkingFileException">Will be thrown when errors occur during setting the marking file</exception>
        /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
        Task<MarkingFileResponse> SetMarkingFileAsync(MarkingFile file, CancellationToken cancellationToken = default);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="config">The configuration for the marking process.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <exception cref="DriverStateException">Thrown if the driver is in an invalid state for this operation.</exception>
        /// <exception cref="MarkingException">Will be thrown when errors occur during marking execution</exception>
        /// <exception cref="SegmentsNotSupportedException">Exception if the system does not support segments</exception>
        /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
        Task<MarkingResponse> MarkAsync(MarkingConfiguration config, CancellationToken cancellationToken = default);
    }
}
