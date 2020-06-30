// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Response of the marking process
    /// </summary>
    public class MarkingResponse : TransmissionResult
    {
        /// <summary>
        /// Successful marking
        /// </summary>
        public MarkingResponse()
        {
        }

        /// <summary>
        /// Faulty marking
        /// </summary>
        /// <param name="errorMessage">Occured error</param>
        public MarkingResponse(string errorMessage) : base(new TransmissionError(errorMessage))
        {
        }
    }
}
