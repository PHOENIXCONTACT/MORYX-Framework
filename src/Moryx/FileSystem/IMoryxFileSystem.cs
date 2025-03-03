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
        /// Write a stream to the file system and represent by the given file
        /// </summary>
        /// <returns>The hash of the written file on the disk</returns>
        Task<string> WriteAsync(MoryxFile file, Stream content);

        /// <summary>
        /// Resolve MORYX file by hash
        /// </summary>
        MoryxFile GetFile(string hash);

        /// <summary>
        /// Read the file content for the MORYX file
        /// </summary>
        Stream OpenStream(MoryxFile file);

        /// <summary>
        /// Return all files stored under a given owner
        /// </summary>
        /// <param name="ownerKey"></param>
        /// <returns></returns>
        MoryxFileTree ReadTreeByOwner(string ownerKey);

        /// <summary>
        /// Remove a file by hash and provided owner key.
        /// Files without owner key can not be removed
        /// </summary>
        bool RemoveFile(MoryxFile file);
    }
}
