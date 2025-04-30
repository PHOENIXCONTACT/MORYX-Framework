using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.FileSystem
{
    public static class MoryxFileSystemExtensions
    {
        public static MoryxFile FindFile(this MoryxFileTree fileTree, string hash)
        {
            if(fileTree.Hash  == hash) 
                return fileTree;

            return FindFile(fileTree.Files, hash);
        }

        public static MoryxFile FindFile(this IReadOnlyList<MoryxFile> files, string hash)
        {
            foreach (var file in files)
            {
                if (file.Hash == hash)
                    return file;

                if (file is not MoryxFileTree subTree)
                    continue;

                var match = FindFile(subTree.Files, hash);
                if (match != null)
                    return match;
            }

            return null;
        }

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

        public static Stream OpenStream(this IMoryxFileSystem fileSystem, MoryxFile file) => fileSystem.OpenStream(file.Hash);
    }
}
