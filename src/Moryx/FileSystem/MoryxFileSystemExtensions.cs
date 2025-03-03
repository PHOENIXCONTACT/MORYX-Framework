using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.FileSystem
{
    public static class MoryxFileSystemExtensions
    {
        public static Task<MoryxFile> WriteBlobAsync(this IMoryxFileSystem fileSystem, Stream stream)
        {
            return fileSystem.WriteAsync(new MoryxFile 
            { 
                FileName = Guid.NewGuid().ToString(),
                MimeType = "application/octet-stream"
            }, stream);
        }

        public static Task<MoryxFile> WriteTreeAsync(this IMoryxFileSystem fileSystem, MoryxFileTree tree)
        {
            return fileSystem.WriteAsync(tree, null);
        }
    }
}
