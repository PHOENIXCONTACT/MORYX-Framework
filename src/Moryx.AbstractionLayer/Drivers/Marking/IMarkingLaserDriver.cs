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
        Task<MarkingFileResponse> SetMarkingFileAsync(MarkingFile file);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="config">The configuration for the marking process.</param>
        Task<MarkingResponse> MarkAsync(MarkingConfiguration config);

        /// <summary>
        /// Will be fired if an error occured
        /// </summary>
        event EventHandler<NotificationResponse> ErrorOccured;

        /// <summary>
        /// Will be fired of a warning occured
        /// </summary>
        event EventHandler<NotificationResponse> WarningOccured;
    }
}
