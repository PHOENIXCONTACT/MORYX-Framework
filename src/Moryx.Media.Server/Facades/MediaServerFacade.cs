// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Media.Server.Endpoint;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Media.Server.Facades
{
    internal class MediaServerFacade : IFacadeControl, IMediaServer
    {
        #region Dependencies

        public IContentManager ContentManager { get; set; }

        public ModelConverter ModelConverter { get; set; }

        #endregion

        #region IFacadeControl

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public Action ValidateHealthState { get; set; }

        #endregion

        #region IMediaServer
        public IReadOnlyList<ContentDescriptor> GetDescriptors()
        {
            ValidateHealthState();
            return ContentManager.GetDescriptors();
        }

        public ContentDescriptor GetDescriptor(Guid contentId)
        {
            ValidateHealthState();
            return ContentManager.GetDescriptor(contentId);
        }

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

        public Task<ContentAddingInfo> AddMaster(string fileName, Stream contentStream)
        {
            ValidateHealthState();
            return ContentManager.AddMaster(fileName, contentStream);
        }

        public Task<ContentAddingInfo> AddVariant(Guid contentId, string variantName, string fileName, Stream contentStream)
        {
            ValidateHealthState();
            return ContentManager.AddVariant(contentId, variantName, fileName, contentStream);
        }

        public bool RemoveContent(Guid contentId, string variantName)
        {
            ValidateHealthState();
            return ContentManager.RemoveContent(contentId, variantName);
        }

        public string GetFileUrl(ContentDescriptor content, VariantDescriptor variant)
        {
            return ModelConverter.GetUrl(InternalConstants.FileUriTemplate, content.Id, variant.Name);
        }

        public string GetPreviewUrl(ContentDescriptor content, VariantDescriptor variant)
        {
            return ModelConverter.GetUrl(InternalConstants.FileUriTemplate, content.Id, variant.Name);
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

        #endregion
    }
}

