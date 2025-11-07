// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
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
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        /// <exception cref="MarkingFileException">Will be thrown when errors occur during setting the marking file</exception>
        Task<MarkingFileResponse> SetMarkingFileAsync(MarkingFile file);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="config">The configuration for the marking process.</param>
        /// <exception cref="DriverStateException">Will be thrown when the driver is in wrong state</exception>
        /// <exception cref="MarkingException">Will be thrown when errors occur during marking execution</exception>
        /// <exception cref="SegmentsNotSupportedException">Exception if the system does not support segments</exception>
        Task<MarkingResponse> MarkAsync(MarkingConfiguration config);
    }
}
