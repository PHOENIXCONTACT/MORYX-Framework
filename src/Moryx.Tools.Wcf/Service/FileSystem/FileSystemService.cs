// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moryx.Container;
using Moryx.Logging;

namespace Moryx.Tools.Wcf.FileSystem
{
    /// <summary>
    /// Handles the web folder to upload and delete files in that folder.
    /// </summary>
    public class FileSystemService : SimpleStreamingServiceBase, IFileSystemService, ILoggingComponent
    {
        /// <summary>
        /// Constructor for the file system service.
        /// </summary>
        public FileSystemService()
        {
            WebRoot = Root = "%TEMP%";
        }

        protected string Root { get; set; }

        /// <summary>
        /// Get the content of the path from the parameters.
        /// </summary>
        /// <param name="relativePath">The relative path of the folder.</param>
        /// <returns>List of <see cref="FileModel"/> of the path.</returns>
        public List<FileModel> GetContentOfPath(string relativePath)
        {
            var targetPath = BuildAbsoluteRootPath(relativePath);
            var content = new List<FileModel>();

            if (!Directory.Exists(targetPath)) 
                return content;

            content.AddRange(Directory.EnumerateDirectories(targetPath)
                .Select(directoryPath => new DirectoryInfo(directoryPath))
                .OrderBy(directoryInfo => directoryInfo.Name)
                .Select(directoryInfo => new FileModel
                {
                    FileName = directoryInfo.Name,
                    CreationDate = directoryInfo.CreationTime, // maybe replace me with LastWriteTime
                    Type = EFileType.Directory
                }));

            content.AddRange(Directory.EnumerateFiles(targetPath)
                .Select(directoryPath => new FileInfo(directoryPath))
                .OrderBy(fileInfo => fileInfo.Name)
                .Select(fileInfo => new FileModel
                {
                    FileName = fileInfo.Name,
                    FileSize = fileInfo.Length,
                    CreationDate = fileInfo.CreationTime, // maybe replace me with LastWriteTime
                    Type = EFileType.File,
                    MimeType = GetMimeType(fileInfo.Name)
                }));

            return content;
        }

        /// <summary>
        /// Upload a file to the web folder.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>True: file uploaded, false: an error occured while uploading. File was not uploaded.</returns>
        public bool UploadFile(RemoteFile file)
        {
            var uploadFolder = BuildAbsoluteRootPath(string.IsNullOrEmpty(file.RelativeTargetPath) ? "" : file.RelativeTargetPath);
            var filePath = Path.Combine(uploadFolder, file.FileName);

            FileStream targetStream = null;
            try
            {
                targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                // save to output stream
                targetStream.Write(Convert.FromBase64String(file.Base64File), 0, (int)file.FileSize);
                targetStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to upload file");
                return false;
            }
            finally
            {
                targetStream?.Dispose();
            }
        }

        /// <summary>
        /// Delete a file on the given path.
        /// </summary>
        /// <param name="relativePath">relative path to the file which should be deleted.</param>
        /// <returns>True: file deleted, false: an error occured and the file was not deleted.</returns>
        public bool DeleteFile(string relativePath)
        {
            var targetPath = BuildAbsoluteRootPath(relativePath);

            if (!File.Exists(targetPath))
                return false;
            try
            {
                File.Delete(targetPath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to delete file on local web folder");
                return false;
            }
        }

        protected string BuildAbsoluteRootPath(string relativePath)
        {
            return $"{Root}\\{relativePath}";
        }


        /// <summary>
        /// Logger instance named "WcfHosting". Injected by castle.
        /// </summary>
        [UseChild("WcfHosting")]
        public IModuleLogger Logger { get; set; }
    }
}
