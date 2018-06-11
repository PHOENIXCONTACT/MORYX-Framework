using System;
using System.IO;
using Marvin.AbstractionLayer;
using static System.IO.File;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Base parameters for a file import
    /// </summary>
    public class FileImportParameters : IFileImportParameters
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
        /// Read file depending on wether it is a base64 encoded string or a filepath
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