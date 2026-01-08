// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Class representing a marking file for laser printing devices
    /// </summary>
    public class MarkingFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkingFile"/> class.
        /// </summary>
        public MarkingFile(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// File to mark with the laser
        /// </summary>
        public byte[] FileBytes { get; private set; }
    }
}
