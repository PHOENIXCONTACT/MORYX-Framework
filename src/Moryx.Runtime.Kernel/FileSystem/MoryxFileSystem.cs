using Microsoft.Extensions.Logging;
using Moryx.FileSystem;
using Moryx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Runtime.Kernel.FileSystem
{
    internal class MoryxFileSystem : IMoryxFileSystem
    {
        private string _fsDirectory;
        private string _ownerFilesDirectory;
        private readonly ILogger _logger;

        public MoryxFileSystem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(MoryxFileSystem));
        }

        public void SetBasePath(string basePath = "fs")
        {
            _fsDirectory = Path.Combine(Directory.GetCurrentDirectory(), basePath);
            _ownerFilesDirectory = Path.Combine(_fsDirectory, "owners");
        }

        public async Task<string> WriteBlob(Stream stream)
        {
            var hashPath = HashPath.FromStream(stream);

            // Create directory if necessary
            var targetPath = hashPath.DirectoryPath(_fsDirectory);
            try
            {
                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);
            }
            catch (Exception e)
            {
                throw LoggedException(e, _logger, _fsDirectory);
            }

            var fileName = hashPath.FilePath(_fsDirectory);
            if (File.Exists(fileName))
                return hashPath.Hash;

            // Write file
            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Create);
                await stream.CopyToAsync(fileStream);
                fileStream.Flush();
                stream.Position = 0;
            }
            catch (Exception e)
            {
                throw LoggedException(e, _logger, fileName);
            }

            return hashPath.Hash;
        }
        public Task<string> WriteTree(IReadOnlyList<MoryxFileMetadata> metadata)
        {
            // Convert metadata to lines 
            var lines = metadata.Select(md => md.ToString()).ToList();
            var stream = new MemoryStream();
            using (var sw = new StreamWriter(stream))
            {
                foreach (var line in lines)
                    sw.WriteLine(line);
                sw.Flush();
            }

            return WriteBlob(stream);
        }

        public async Task<string> WriteBlob(Stream fileStream, string ownerKey, MoryxFileMetadata metadata)
        {
            // Create file first
            var hash = await WriteBlob(fileStream);
            metadata.Hash = hash;

            // Read current owner tree
            var ownerFile = Path.Combine(_ownerFilesDirectory, ownerKey);
            var ownerTree = File.ReadAllText(ownerFile);
            var tree = ReadExtensibleTree(ownerTree);

            // Add to owner tree and write new
            tree.Add(metadata);
            var treeHash = WriteTree(tree);

            // Update owner file
            var ownerFilePath = Path.Combine(_ownerFilesDirectory, ownerKey);
            if (File.Exists(ownerFilePath))
                await File.WriteAllLinesAsync(ownerFilePath, new[] { hash });
            else
                await File.AppendAllLinesAsync(ownerFilePath, new[] { hash });

            return hash;
        }

        public Stream ReadBlob(string hash)
        {
            var path = HashPath.FromHash(hash).FilePath(_fsDirectory);
            return File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read) : null;
        }

        public IReadOnlyList<MoryxFileMetadata> ReadTree(string hash) => ReadExtensibleTree(hash);


        public IReadOnlyList<MoryxFileMetadata> ReadTreeByOwner(string ownerKey)
        {
            // read hash from owner file
            var ownerFile = Path.Combine(_ownerFilesDirectory, ownerKey);
            var ownerTree = File.ReadAllText(ownerFile);

            return ReadExtensibleTree(ownerTree);
        }

        private List<MoryxFileMetadata> ReadExtensibleTree(string hash)
        {
            // Read tree from hash
            var stream = ReadBlob(hash);
            var metadata = new List<MoryxFileMetadata>();
            using (var sr = new StreamReader(stream))
            {
                var line = sr.ReadLine();
                metadata.Add(MoryxFileMetadata.FromLine(line));
            }

            return metadata;
        }

        public bool RemoveFile(string hash, string ownerKey)
        {
            if (!IsOwner(hash, ownerKey))
                return false;

            // Delete file if found
            var hashPath = HashPath.FromHash(hash);
            var filePath = hashPath.FilePath(_fsDirectory);
            if (!File.Exists(filePath))
                return false;
            RemoveFile(filePath, _logger);

            // Check if subdirectory is empty and remove
            var directory = hashPath.DirectoryPath(_fsDirectory);
            CleanUpDirectory(directory, _logger);

            // TODO: Remove file from owner list

            return true;
        }

        private bool IsOwner(string hash, string ownerFile)
        {
            var ownerFilePath = Path.Combine(_ownerFilesDirectory, ownerFile);
            using (var reader = new StreamReader(ownerFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(searchLine))
                        return true;
                }
            }
            return false;
        }

        private void RemoveFile(string filePath, ILogger logger)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                throw LoggedException(e, logger, filePath);
            }
        }

        private void CleanUpDirectory(string directoryPath, ILogger logger)

        {
            try
            {
                if (Directory.GetFiles(directoryPath).Length == 0)
                    Directory.Delete(directoryPath);
            }
            catch (Exception e)
            {
                throw LoggedException(e, logger, directoryPath);
            }
        }

        private Exception LoggedException(Exception e, ILogger logger, string cause)
        {
            switch (e)
            {
                case UnauthorizedAccessException unauthorizedAccessException:
                    logger.LogError("Error: {0}. You do not have the required permission to manipulate the file {1}.", e.Message, cause); // ToDo
                    return unauthorizedAccessException;
                case ArgumentException argumentException:
                    logger.LogError("Error: {0}. The path {1} contains invalid characters such as \", <, >, or |.", e.Message, cause);
                    return argumentException;
                case IOException iOException:
                    logger.LogError("Error: {0}. An I/O error occurred while opening the file {1}.", e.Message, cause);
                    return iOException;
                default:
                    logger.LogError("Unspecified error on file system access: {0}", e.Message);
                    return e;
            }
        }
    }
}
