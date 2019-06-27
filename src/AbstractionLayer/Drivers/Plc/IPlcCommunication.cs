using System;

namespace Marvin.AbstractionLayer.Drivers.Plc
{
    /// <summary>
    /// Interface for communication with the plc
    /// </summary>
    public interface IPlcCommunication
    {
        /// <summary>
        /// Reference to the underlying driver of this communication
        /// </summary>
        IPlcDriver Driver { get; }

        /// <summary>
        /// Identifier of this communication
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Send message to plc and don't care for a response
        /// </summary>
        void Send(IQuickCast message);

        /// <summary>
        /// Send object to the PLC and await a transmission confirmation
        /// </summary>
        void Send(IQuickCast message, DriverResponse<PlcTransmissionResult> response);

        /// <summary>
        /// Event raised when the plc driver receives a message
        /// </summary>
        event EventHandler<IQuickCast> Received;
    }
}