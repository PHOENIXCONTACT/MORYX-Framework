// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Response of the marking file setup
    /// </summary>
    public class MarkingFileResponse : TransmissionResult
    {
        /// <summary>
        /// Successful marking file setup
        /// </summary>
        public MarkingFileResponse() : base()
        {
        }

        /// <summary>
        /// Faulty marking file setup
        /// </summary>
        /// <param name="errorMessage">Occured error</param>
        public MarkingFileResponse(string errorMessage) : base(new TransmissionError(errorMessage))
        {
        }
    }
}
