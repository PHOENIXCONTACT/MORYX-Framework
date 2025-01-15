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
        Task<string> WriteFile(Stream fileStream);

        /// <summary>
        /// Read the file by passing the file system hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Stream ReadFile(string hash);
    }
}
