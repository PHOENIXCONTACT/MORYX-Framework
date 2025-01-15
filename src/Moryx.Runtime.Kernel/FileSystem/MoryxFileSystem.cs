using Microsoft.Extensions.Logging;
using Moryx.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Runtime.Kernel.FileSystem
{
    internal class MoryxFileSystem : IMoryxFileSystem
    {
        private string _fsDirectory;
        private readonly ILogger _logger;

        public MoryxFileSystem(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(MoryxFileSystem));
        }

        public void SetBasePath(string basePath = "fs")
        {
            _fsDirectory = Path.Combine(Directory.GetCurrentDirectory(), basePath);
        }

        public Stream ReadFile(string hash)
        {
            var path = HashPath.FromHash(hash).FilePath(_fsDirectory);           
            return File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read) : null;
        }

        public async Task<string> WriteFile(Stream stream)
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
