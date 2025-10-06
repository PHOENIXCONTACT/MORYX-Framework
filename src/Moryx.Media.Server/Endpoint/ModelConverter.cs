// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;

namespace Moryx.Media.Server.Endpoint
{
    [Component(LifeCycle.Singleton)]
    internal class ModelConverter
    {
        public IConfigManager RuntimeConfigManager { get; set; }

        public string GetUrl(string template, Guid contentId, string variantName)
        {
            var portConfig = RuntimeConfigManager.GetConfiguration<PortConfig>();
            var httpPort = portConfig.HttpPort != 80 ? $":{portConfig.HttpPort}" : string.Empty;
#if USE_WCF
            var endpoint = ModuleConfig.HostConfig.Endpoint;
#else
            var endpoint = MediaService.Endpoint;
#endif
            var baseAddress = new Uri($"http://{portConfig.Host}{httpPort}/{endpoint}");

            var namedUri = baseAddress + "/" + template
                .Replace("{contentId}", contentId.ToString())
                .Replace("{variantName}", variantName);

            return namedUri;
        }

        public VariantDescriptorModel ConvertVariant(VariantDescriptor variantDescriptor, ContentDescriptor contentDescriptor)
        {
            if (variantDescriptor == null)
                return null;

            return new VariantDescriptorModel
            {
                Name = variantDescriptor.Name,
                IsMaster = variantDescriptor.Name == MediaConstants.MasterName,
                Size =variantDescriptor.Size,
                FileHash = variantDescriptor.FileHash,
                CreationDate = variantDescriptor.CreationDate,
                MimeType = variantDescriptor.MimeType,
                Extension = variantDescriptor.Extension,
                Preview = ConvertPreview(variantDescriptor, contentDescriptor),
                FileUrl = GetUrl(InternalConstants.FileUriTemplate, contentDescriptor.Id, variantDescriptor.Name)
            };
        }

        public PreviewDescriptorModel ConvertPreview(VariantDescriptor variantDescriptor, ContentDescriptor contentDescriptor)
        {
            var previewDescriptor = variantDescriptor.Preview;
            return new PreviewDescriptorModel
            {
                FileHash = previewDescriptor.FileHash,
                MimeType = previewDescriptor.MimeType,
                Size = previewDescriptor.Size,
                PreviewState = previewDescriptor.State,
                Extension = previewDescriptor.Extension,
                FileUrl = GetUrl(InternalConstants.PreviewUriTemplate, contentDescriptor.Id, variantDescriptor.Name)
            };
        }

        public ContentAddingInfoModel ConvertAddingInfo(ContentAddingInfo contentAddingInfo)
        {
            return new()
            {
                ContentDescriptor = ConvertContent(contentAddingInfo.Descriptor),
                VariantDescriptor = ConvertVariant(contentAddingInfo.Variant, contentAddingInfo.Descriptor),
                Result = contentAddingInfo.Result
            };
        }

        public ContentDescriptorModel ConvertContent(ContentDescriptor contentDescriptor)
        {
            if (contentDescriptor == null)
                return null;

            var variants = contentDescriptor.Variants.Select(variant => ConvertVariant(variant, contentDescriptor)).ToList();
            return new ContentDescriptorModel
            {
                Id = contentDescriptor.Id,
                Name = contentDescriptor.Name,
                Variants = variants
            };
        }
    }
}
