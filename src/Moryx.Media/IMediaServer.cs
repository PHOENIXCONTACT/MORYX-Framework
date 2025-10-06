// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Media
{
    /// <summary>
    /// Public API of the MediaServer. Reachable via the facade.
    /// </summary>
    public interface IMediaServer
    {
        /// <summary>
        /// Get all ContentDescriptors from the MediaServer.
        /// </summary>
        IReadOnlyList<ContentDescriptor> GetAll();

        /// <summary>
        /// Get all ContentDescriptors from the MediaServer.
        /// </summary>
        IReadOnlyList<ContentDescriptor> GetAll(Predicate<ContentDescriptor> predicate);

        /// <summary>
        /// Get the ContentDescriptor with the given contentId.
        /// </summary>
        ContentDescriptor Get(Guid contentId);

        /// <summary>
        /// Get the VariantDescriptor with the "variantName" from the ContentDescriptor with the "contentId".
        /// </summary>
        VariantDescriptor GetVariant(Guid contentId, string variantName);

        /// <summary>
        /// Get the file stream from the file described by the fileDescriptor.
        /// </summary>
        /// <returns>Returns null if the file does not exist.</returns>
        Stream GetStream(FileDescriptor fileDescriptor);

        /// <summary>
        /// Adds a new file to the MediaServer.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to create this directory.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the stream does not support reading or seeking, or the fileStream does not support writing.</exception>

        Task<ContentAddingInfo> AddMaster(string fileName, Stream contentStream);

        /// <summary>
        /// Adds a new file to the MediaServer. The new file is a variant of the Master-Content.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to create this directory.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="System.NotSupportedException">Thrown when the stream does not support reading or seeking, or the fileStream does not support writing.</exception>
        Task<ContentAddingInfo> AddVariant(Guid contentId, string variantName, string fileName, Stream contentStream);

        /// <summary>
        /// Removes the content with the given id from the MediaServer.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to delete this directory.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>

        bool RemoveContent(Guid contentId);

        /// <summary>
        /// Removes the varient with the given name and id from the MediaServer.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the path is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurred while opening the file or if the directory specified by path is a file.</exception>
        /// <exception cref="System.Security.SecurityException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Thrown when path specified a file that is read-only or if the caller does not have the required permission to delete this directory.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown when the specified path, file name, or both exceed the system-defined maximum length.</exception>

        bool RemoveVariant(Guid contentId, string variantName);
    
        /// <summary>
        /// List of file types allowed to be uploaded
        /// </summary>
        string[] GetSupportedFileTypes();

        /// <summary>
        /// Return the maximum allowed file size to be uploaded in MB
        /// </summary>
        int FileSizeLimitInMb();
    }
}
