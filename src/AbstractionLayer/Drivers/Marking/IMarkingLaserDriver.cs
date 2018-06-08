using System;
using System.Collections.Generic;
using Marvin.AbstractionLayer.Hardware;

namespace Marvin.AbstractionLayer.Drivers.Marking
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
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="callback">The callback which will be executed after the marking process</param>
        [Obsolete("Mark(DriverResponse<MarkingResponse> callback) is obsolete.  Use Mark(MarkingConfiguration config, DriverResponse<MarkingResponse> callback) instead.")]
        void Mark(DriverResponse<MarkingResponse> callback);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="variables">Marking variabes to set before start marking</param>
        /// <param name="callback">The callback which will be executed after the marking process</param>
        [Obsolete("Mark(IDictionary<string, string> variables, DriverResponse<MarkingResponse> callback) is obsolete.  Use Mark(MarkingConfiguration config, DriverResponse<MarkingResponse> callback) instead.")]
        void Mark(IDictionary<string, string> variables, DriverResponse<MarkingResponse> callback);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// The marking process will be started WHITHOUT variables. 
        /// </summary>
        /// <param name="segment">The segment for the next marking process. Known segments: Trumpf: 1-n, Rofin: 1-n, Foba: 0-255</param>
        /// <param name="callback">The callback which will be executed after the marking process</param>
        /// <exception cref="SegmentsNotSupportedException">Will be thrown if segments are not supported</exception>
        [Obsolete("Mark(int segment, DriverResponse<MarkingResponse> callback) is obsolete.  Use Mark(MarkingConfiguration config, DriverResponse<MarkingResponse> callback) instead.")]
        void Mark(int segment, DriverResponse<MarkingResponse> callback);

        /// <summary>
        /// Will start the marking process and executes the given callback after finish
        /// </summary>
        /// <param name="segment">The segment for the next marking process. Known segments: Trumpf: 1-n, Rofin: 1-n, Foba: 0-255</param>
        /// <param name="callback">The callback which will be executed after the marking process</param>
        /// <param name="variables">Marking variabes to set before start marking</param>
        /// <exception cref="SegmentsNotSupportedException">Will be thrown if segments are not supported</exception>
        [Obsolete("Mark((int segment, IDictionary<string, string> variables, DriverResponse<MarkingResponse> callback) is obsolete.  Use Mark(MarkingConfiguration config, DriverResponse<MarkingResponse> callback) instead.")]
        void Mark(int segment, IDictionary<string, string> variables, DriverResponse<MarkingResponse> callback);

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