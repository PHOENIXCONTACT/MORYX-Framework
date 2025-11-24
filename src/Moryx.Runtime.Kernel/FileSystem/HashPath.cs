using Microsoft.Extensions.Logging;
using Moryx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Runtime.Kernel.FileSystem
{
    internal class HashPath
    {
        public string Hash { get; private set; }

        public string DirectoryName { get; private set; }

        public string FileName { get; private set; }

        private HashPath()
        {
        }

        public static HashPath FromStream(Stream stream) =>
            BuildPath(HashFromStream(stream));

        public static HashPath FromHash(string hash) =>
            BuildPath(hash);

        public string FilePath(string storagePath) =>
            Path.Combine(storagePath, DirectoryName, FileName);

        public string DirectoryPath(string storagePath) =>
            Path.Combine(storagePath, DirectoryName);

        private static HashPath BuildPath(string hash)
        {
            return new HashPath
            {
                Hash = hash,
                DirectoryName = hash.Substring(0, 2),
                FileName = hash.Substring(2)
            };
        }

        private static string HashFromStream(Stream stream)
        {
            string name;
            using (var hashing = SHA256.Create())
            {
                stream.Position = 0;

                var hash = hashing.ComputeHash(stream);
                name = BitConverter.ToString(hash).Replace("-", string.Empty);

                stream.Position = 0;
            }

            return name;
        }
    }
}
