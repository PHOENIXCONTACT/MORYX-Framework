// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Plc
{
    /// <summary>
    /// Result of plc message transmission
    /// </summary>
    public class PlcTransmissionResult : TransmissionResult
    {
        /// <summary>
        /// Message that should be transmitted
        /// </summary>
        public object OriginalMessage { get; private set; }

        /// <summary>
        /// Faulty transmission
        /// </summary>
        public PlcTransmissionResult(object originalMessage, string error) : base(new TransmissionError(error))
        {
            OriginalMessage = originalMessage;
        }

        /// <summary>
        /// Successful transmission
        /// </summary>
        /// <param name="originalMessage"></param>
        public PlcTransmissionResult(object originalMessage)
        {
            OriginalMessage = originalMessage;
        }
    }
}
