// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Moryx.Media.Server;

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
            var nameBuilder = new StringBuilder(hash.Length * 2);
            foreach (var hashByte in hash)
            {
                nameBuilder.AppendFormat("{0:X2}", hashByte);
            }
            name = nameBuilder.ToString();

            stream.Position = 0;
        }

        return name;
    }

    /// <summary>
    /// Saves a stream to a specified storage path.
    /// </summary>
    /// <param name="stream">The stream to save.</param>
    /// <param name="storagePath">The storage path where the stream will be saved.</param>
    /// <param name="logger">The logger used to log process and error information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
    /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
    /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to create this directory.</exception>
    /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.NotSupportedException">Thrown when the stream does not support reading or seeking, or the fileStream does not support writing.</exception>
    public async Task SaveStream(Stream stream, string storagePath, IModuleLogger logger)
    {
        var targetPath = DirectoryPath(storagePath);

        try
        {
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
        }
        catch (Exception e)
        {
            throw LoggedException(e, logger, storagePath);
        }

        var fileName = FilePath(storagePath);
        if (File.Exists(fileName))
            return;

        try
        {
            using var fileStream = new FileStream(fileName, FileMode.Create);
            await stream.CopyToAsync(fileStream);
            fileStream.Flush();
            stream.Position = 0;
        }
        catch (Exception e)
        {
            throw LoggedException(e, logger, fileName);
        }
    }

    private Exception LoggedException(Exception e, IModuleLogger logger, string cause)
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

    /// <summary>
    /// Deletes a file at a specified storage path and deletes the directory if it is empty.
    /// </summary>
    /// <param name="storagePath">The storage path of the file to be deleted.</param>
    /// <param name="logger">The logger used to log process and error information.</param>
    /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
    /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
    /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to delete this directory.</exception>
    /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>
    public void DeleteFile(string storagePath, IModuleLogger logger)
    {
        RemoveFile(storagePath, logger);

        CleanUpDirectory(storagePath, logger);
    }

    private void RemoveFile(string storagePath, IModuleLogger logger)
    {
        var filePath = FilePath(storagePath);
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception e)
        {
            throw LoggedException(e, logger, filePath);
        }
    }

    private void CleanUpDirectory(string storagePath, IModuleLogger logger)
    {
        var directoryPath = DirectoryPath(storagePath);
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

}