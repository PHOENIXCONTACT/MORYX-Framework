// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using static System.IO.File;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Base parameters for a file import
    /// </summary>
    public class FileImportParameters
    {
        /// <summary>
        /// File extension of the file
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// File that shall be imported
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Read file depending on whether it is a base64 encoded string or a filepath
        /// </summary>
        public Stream ReadFile()
        {
            // Try to read as file stream
            if (Exists(File))
            {
                return Open(File, FileMode.Open);
            }

            // Read as Base64 stream
            var bytes = Convert.FromBase64String(File);
            return new MemoryStream(bytes);
        }
    }
}
