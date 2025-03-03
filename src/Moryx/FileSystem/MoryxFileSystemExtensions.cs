using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.FileSystem
{
    public static class MoryxFileSystemExtensions
    {
        public static Task<string> WriteBlobAsync(this IMoryxFileSystem fileSystem, Stream stream)
        {
            return fileSystem.WriteAsync(new MoryxFile 
            { 
                FileName = Guid.NewGuid().ToString(),
                MimeType = "application/octet-stream"
            }, stream);
        }

        public static Task<string> WriteTreeAsync(this IMoryxFileSystem fileSystem, MoryxFileTree tree)
        {
            return fileSystem.WriteAsync(tree, null);
        }
    }
}
