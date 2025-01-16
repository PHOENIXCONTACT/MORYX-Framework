using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.FileSystem
{
    /// <summary>
    /// Common file system interface across MORYX modules and components. 
    /// </summary>
    public interface IMoryxFileSystem
    {
        /// <summary>
        /// Write a file to the file system and receive the hash to access it later
        /// </summary>
        Task<string> WriteBlob(Stream fileStream);

        /// <summary>
        /// Write a file to the file system and receive the hash to access it later
        /// </summary>
        Task<string> WriteBlob(Stream fileStream, string ownerKey, MoryxFileMetadata metadata);

        /// <summary>
        /// Write a file to the file system and receive the hash to access it later
        /// </summary>
        Task<string> WriteTree(IReadOnlyList<MoryxFileMetadata> metadata);

        /// <summary>
        /// Read the file by passing the file system hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Stream ReadBlob(string hash);

        /// <summary>
        /// Read a tree file and return the listed files
        /// </summary>
        IReadOnlyList<MoryxFileMetadata> ReadTree(string hash);

        /// <summary>
        /// Return all files stored under
        /// </summary>
        /// <param name="ownerKey"></param>
        /// <returns></returns>
        IReadOnlyList<MoryxFileMetadata> ReadTreeByOwner(string ownerKey);

        /// <summary>
        /// Remove a file by hash and provided owner key.
        /// Files without owner key can not be removed
        /// </summary>
        bool RemoveFile(string hash, string ownerKey);
    }
}
