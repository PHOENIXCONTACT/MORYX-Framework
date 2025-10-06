// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Media.Server
{
    internal interface IContentManager : IPlugin
    {
        /// <summary>
        /// Get all ContentDescriptors from the MediaServer.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ContentDescriptor> GetDescriptors();

        /// <summary>
        /// Get all ContentDescriptors with index between "start"Index and  "start"Index plus the "offset".
        /// </summary>
        IReadOnlyList<ContentDescriptor> GetDescriptors(uint start, uint offset);

        /// <summary>
        /// Get the ContentDescriptor with the given contentId.
        /// </summary>
        ContentDescriptor GetDescriptor(Guid contentId);

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
        Task<ContentAddingInfo> AddMaster(string fileName, Stream contentStream);

        /// <summary>
        /// Adds a new file to the MediaServer. The new file is a variant of the Master-Content.
        /// </summary>
        Task<ContentAddingInfo> AddVariant(Guid contentId, string variantName, string fileName, Stream contentStream);

        /// <summary>
        /// Removes the content with the given name and id from the MediaServer.
        /// </summary>
        bool RemoveContent(Guid contentId, string variantName);

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

