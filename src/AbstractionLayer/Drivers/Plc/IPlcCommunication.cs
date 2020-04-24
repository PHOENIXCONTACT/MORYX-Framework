// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer.Drivers.Message;

namespace Marvin.AbstractionLayer.Drivers.Plc
{
    /// <summary>
    /// Interface for communication with the plc
    /// </summary>
    public interface IPlcCommunication : IMessageCommunication
    {
        /// <summary>
        /// Send object to the PLC and await a transmission confirmation
        /// </summary>
        void Send(object message, DriverResponse<PlcTransmissionResult> response);
    }
}
