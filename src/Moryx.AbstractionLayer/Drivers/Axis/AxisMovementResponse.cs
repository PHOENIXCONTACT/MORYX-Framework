// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis
{
    /// <summary>
    /// Response of the marking file setup
    /// </summary>
    public class AxisMovementResponse : TransmissionResult
    {
        /// <summary>
        /// Successfull movement of the axis
        /// </summary>
        public AxisMovementResponse() : base()
        {

        }

        /// <summary>
        /// Faulty movement of the axis
        /// </summary>
        /// <param name="errorMessage">Occured error</param>
        public AxisMovementResponse(string errorMessage) : base(new TransmissionError(errorMessage))
        {

        }
    }
}
