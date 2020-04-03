// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Exception if the marking driver does not support segments
    /// </summary>
    public class SegmentsNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentsNotSupportedException"/> class.
        /// </summary>
        public SegmentsNotSupportedException() : base("The driver does not support segments")
        {

        }
    }
}
