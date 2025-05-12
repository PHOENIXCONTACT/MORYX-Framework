using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Logging;
using Moryx.FileSystem;
using Moryx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Runtime.Kernel.FileSystem
{
    internal class LocalFileSystem : IMoryxFileSystem
    {
        private string _fsDirectory;
        private string _ownerFilesDirectory;
        private readonly ILogger _logger;

        private readonly List<MoryxFileTree> _ownerTrees = new List<MoryxFileTree>();

        public LocalFileSystem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(LocalFileSystem));
        }

        public void SetBasePath(string basePath = "fs")
        {
            _fsDirectory = Path.Combine(Directory.GetCurrentDirectory(), basePath);
            _ownerFilesDirectory = Path.Combine(_fsDirectory, "owners");
        }

        public void LoadTrees()
        {
            // Load all trees from the owner directory
            var ownerFiles = Directory.EnumerateFiles(_ownerFilesDirectory);
            foreach (var ownerFile in ownerFiles)
            {
                var treeHash = File.ReadAllText(ownerFile);
                var ownerTree = new MoryxFileTree
                {
                    FileName = ownerFile,
                    Hash = treeHash,
                };
                ReadExtensibleTree(ownerTree);
            }
        }

        public MoryxFile GetFile(string hash)
        {
            return _ownerTrees.FindFile(hash);
        }

        public async Task<string> WriteAsync(MoryxFile file, Stream content)
        {
            if (file is MoryxFileTree fileTree)
                return await WriteTreeAsync(fileTree);
            else if (content != null)
                return await WriteBlobAsync(file, content);

            throw new ArgumentException("For all files except trees the content stream must be given");
        }

        private async Task<string> WriteTreeAsync(MoryxFileTree tree)
        {
            // Convert metadata to lines 
            var stream = new MemoryStream();
            using (var sw = new StreamWriter(stream))
            {
                foreach (var line in tree.Files.Select(FileToLine))
                    sw.WriteLine(line);
                await sw.FlushAsync();
            }

            tree.Hash = await StreamToDiskAsync(stream);

            // Update parent or owner file
            if (tree.ParentTree == null)
            {
                var ownerFile = Path.Combine(_ownerFilesDirectory, tree.FileName);
                File.WriteAllText(ownerFile, tree.Hash);
            }
            else
            {
                await WriteTreeAsync(tree.ParentTree);
            }

            return tree.Hash;
        }

        private async Task<string> WriteBlobAsync(MoryxFile file, Stream fileStream)
        {
            // Create file first
            var hash = await StreamToDiskAsync(fileStream);
            file.Hash = hash;

            // Now write the tree recursively 
            await WriteTreeAsync(file.ParentTree);

            return hash;
        }

        private async Task<string> StreamToDiskAsync(Stream stream)
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
                await fileStream.FlushAsync();
                stream.Position = 0;
            }
            catch (Exception e)
            {
                throw LoggedException(e, _logger, fileName);
            }

            return hashPath.Hash;
        }

        public Stream OpenStream(string hash)
        {
            var path = HashPath.FromHash(hash).FilePath(_fsDirectory);
            return File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read) : null;
        }

        public MoryxFileTree ReadTreeByOwner(string ownerKey)
        {
            var existingTree = _ownerTrees.FirstOrDefault(ot => ot.FileName == ownerKey);
            if (existingTree == null)
            {
                existingTree = new MoryxFileTree { FileName = ownerKey };
                _ownerTrees.Add(existingTree);
            }
            return existingTree;
        }

        private MoryxFileTree ReadExtensibleTree(MoryxFileTree tree)
        {
            // Read tree from hash
            var path = HashPath.FromHash(tree.Hash).FilePath(_fsDirectory);
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var file = FileFromLine(line);
                tree.AddFile(file);

                if (file is MoryxFileTree subTree)
                    ReadExtensibleTree(subTree);
            }

            return tree;
        }

        public bool RemoveFile(MoryxFile file)
        {
            // Delete file if found
            var hashPath = HashPath.FromHash(file.Hash);
            var filePath = hashPath.FilePath(_fsDirectory);
            if (!File.Exists(filePath))
                return false;

            RemoveFile(filePath, _logger);

            // Check if subdirectory is empty and remove
            var directory = hashPath.DirectoryPath(_fsDirectory);
            CleanUpDirectory(directory, _logger);

            // Remove file from tree and rewrite
            var parentTree = file.ParentTree;
            parentTree.RemoveFile(file);
            WriteTreeAsync(parentTree).Wait();

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
                    if (line.Contains(hash))
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

        private static string FileToLine(MoryxFile file)
        {
            return $"{file.Mode} {file.FileType.ToString().ToLower()} {file.MimeType} {file.Hash} {file.FileName}";
        }

        private static MoryxFile FileFromLine(string line)
        {
            var parts = line.Split(' ');

            var file = parts[1] == FileType.Blob.ToString().ToLower()
                ? new MoryxFile() : new MoryxFileTree();
            file.Mode = int.Parse(parts[0]);
            file.MimeType = parts[2];
            file.Hash = parts[3];
            file.FileName = string.Join(" ", parts.Skip(4));

            return file;
        }
    }
}
