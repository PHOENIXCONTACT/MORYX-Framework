// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Media.Server.Facades
{
    internal class MediaServerFacade : FacadeBase, IMediaServer
    {
        #region Dependencies

        public IContentManager ContentManager { get; set; }

        #endregion

        public VariantDescriptor GetVariant(Guid contentId, string variantName)
        {
            ValidateHealthState();
            return ContentManager.GetVariant(contentId, variantName);
        }

        public Stream GetStream(FileDescriptor fileDescriptor)
        {
            ValidateHealthState();
            return ContentManager.GetStream(fileDescriptor);
        }

        public Task<ContentAddingInfo> AddMasterAsync(string fileName, Stream contentStream, CancellationToken cancellationToken = default)
        {
            ValidateHealthState();
            return ContentManager.AddMaster(fileName, contentStream);
        }

        public Task<ContentAddingInfo> AddVariantAsync(Guid contentId, string variantName, string fileName, Stream contentStream,
            CancellationToken cancellationToken = default)
        {
            ValidateHealthState();
            return ContentManager.AddVariant(contentId, variantName, fileName, contentStream);
        }

        /// <inheritdoc />
        public IReadOnlyList<ContentDescriptor> GetAll()
        {
            ValidateHealthState();
            return ContentManager.GetDescriptors();
        }

        /// <inheritdoc />
        public IReadOnlyList<ContentDescriptor> GetAll(Predicate<ContentDescriptor> predicate)
        {
            ValidateHealthState();
            return ContentManager.GetDescriptors().Where(c => predicate(c)).ToList();
        }

        /// <inheritdoc />
        public ContentDescriptor Get(Guid contentId)
        {
            ValidateHealthState();
            return ContentManager.GetDescriptor(contentId);
        }

        /// <inheritdoc />
        public bool RemoveContent(Guid contentId)
        {
            ValidateHealthState();
            return ContentManager.RemoveContent(contentId, MediaConstants.MasterName);
        }

        /// <inheritdoc />
        public bool RemoveVariant(Guid contentId, string variantName)
        {
            ValidateHealthState();
            return ContentManager.RemoveContent(contentId, variantName);
        }

        /// <inheritdoc />
        public string[] GetSupportedFileTypes()
        {
            ValidateHealthState();
            return ContentManager.GetSupportedFileTypes();
        }

        /// <inheritdoc />
        public int FileSizeLimitInMb()
        {
            ValidateHealthState();
            return ContentManager.FileSizeLimitInMb();
        }
    }
}

