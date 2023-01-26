// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Driver interface for laser printing devices
    /// </summary>
    public interface IMarkingLaserDriver : IDriver
    {
        /// <summary>
        /// Set up marking file as a preperation for the marking process
        /// </summary>
        void SetMarkingFile(MarkingFile file, DriverResponse<MarkingFileResponse> callback);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="config">The configuration for the marking process.</param>
        /// <param name="callback">The callback which will be executed after the marking process</param>
        void Mark(MarkingConfiguration config, DriverResponse<MarkingResponse> callback);

        /// <summary>
        /// Triggers a message to get the last error
        /// </summary>
        void RequestLastError();

        /// <summary>
        /// Triggers a message to get the last warning
        /// </summary>
        void RequestLastWarning();

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
