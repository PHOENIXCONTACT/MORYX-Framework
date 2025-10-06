// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using Moryx.Container;

#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Media.Server.Endpoint
{
    [Plugin(LifeCycle.Transient, typeof(IMediaService))]
#if USE_WCF
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class MediaService : IMediaService
#else
    [ApiController, Route(Endpoint)]
    [Endpoint(Name = nameof(IMediaService), Version = "4.0.0")]
    internal class MediaService : Controller, IMediaService
#endif
    {
        internal const string Endpoint = "media";

        #region Dependency Injection

        public IContentManager ContentManager { get; set; }

        public ModelConverter ModelConverter { get; set; }

        #endregion

#if !USE_WCF
        [HttpGet("descriptors")]
#endif
        public ContentDescriptorModel[] GetDescriptors()
        {
            var descriptors = ContentManager.GetDescriptors();

            var models = new ContentDescriptorModel[descriptors.Count];
            for (int index = 0; index < descriptors.Count; index++)
                models[index] = ModelConverter.ConvertContent(descriptors[index]);

            return models;
        }

#if !USE_WCF
        [HttpGet("descriptors/query")]
        public ContentDescriptorModel[] GetDescriptorsByPage([FromQuery(Name = "start")] uint start, [FromQuery(Name = "offset")] uint offset)
#else
        public ContentDescriptorModel[] GetDescriptorsByPage(uint start, uint offset)
#endif
        {
            return ContentManager.GetDescriptors(start, offset).Select(ModelConverter.ConvertContent).ToArray();
        }

#if !USE_WCF
        [HttpGet("descriptors/{contentId}")]
#endif
        public ContentDescriptorModel GetDescriptor(string contentId)
        {
            var guid = ConvertGuid(contentId);
            if (guid == Guid.Empty)
                return null;

            var descriptor = ContentManager.GetDescriptor(guid);
            if (descriptor == null)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                return null;
            }

            return ModelConverter.ConvertContent(descriptor);
        }

#if !USE_WCF
        [HttpPost("descriptors")]
#endif
        public async Task<ContentAddingInfoModel> AddContent(ContentUploadRequest uploadRequest)
        {
            ContentAddingInfo addingInfo;

            var buffer = Convert.FromBase64String(uploadRequest.File);
            var ms = new MemoryStream(buffer);

            if (!uploadRequest.ContentId.HasValue)
            {
                addingInfo = await ContentManager.AddMaster(uploadRequest.FileName, ms);
            }
            else
            {
                addingInfo = await ContentManager.AddVariant(uploadRequest.ContentId.Value, uploadRequest.VariantName,
                    uploadRequest.FileName, ms);
            }

            return ModelConverter.ConvertAddingInfo(addingInfo);
        }

#if !USE_WCF
        [HttpDelete("descriptors/{contentId}/{variantName}")]
#endif
        public bool RemoveVariant(string contentId, string variantName)
        {
            var guid = ConvertGuid(contentId);
            if (guid == Guid.Empty)
                return false;

            return ContentManager.RemoveContent(guid, variantName);
        }

#if !USE_WCF
        [HttpGet("descriptors/{contentId}/{variantName}")]
#endif
        public VariantDescriptorModel GetVariant(string contentId, string variantName)
        {
            var guid = ConvertGuid(contentId);
            if (guid == Guid.Empty)
                return null;

            var variant = ContentManager.GetVariant(guid, variantName);
            if (variant == null)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                return null;
            }

            var content = ContentManager.GetDescriptor(guid);
            return ModelConverter.ConvertVariant(variant, content);
        }

#if !USE_WCF
        [HttpGet("descriptors/{contentId}/{variantName}/preview")]
#endif
        public PreviewDescriptorModel GetPreview(string contentId, string variantName)
        {
            var guid = ConvertGuid(contentId);
            if (guid == Guid.Empty)
                return null;

            var content = ContentManager.GetDescriptor(guid);
            var variant = ContentManager.GetVariant(guid, variantName);
            return variant != null ? ModelConverter.ConvertPreview(variant, content) : null;
        }

#if !USE_WCF
        [HttpGet(InternalConstants.FileUriTemplate)]
#endif
        public Stream DownloadVariant(string contentId, string variantName)
        {
            return GetFile(contentId, variantName, FileType.File);
        }

#if !USE_WCF
        [HttpGet(InternalConstants.PreviewUriTemplate)]
#endif
        public Stream DownloadPreview(string contentId, string variantName)
        {
            return GetFile(contentId, variantName, FileType.Preview);
        }

#if !USE_WCF
        [HttpGet("files")]
        public Stream FileSearch([FromQuery] string fileHash = null, [FromQuery] string fileName = null, [FromQuery] string variantName = MediaConstants.MasterName, [FromQuery] string preview = "false")
#else
        public Stream FileSearch(string fileHash = null, string fileName = null, string variantName = MediaConstants.MasterName, string preview = "false")
#endif
        {
            var descriptors = !string.IsNullOrEmpty(fileName)
                ? ContentManager.GetDescriptors().Where(d => d.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                : ContentManager.GetDescriptors();

            var harmVariant = string.IsNullOrEmpty(variantName) ? MediaConstants.MasterName : variantName;
            var variants = descriptors.SelectMany(v => v.Variants)
                .Where(v => v.Name.Equals(harmVariant, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(fileHash))
                variants = variants.Where(v => v.FileHash.Equals(fileHash, StringComparison.OrdinalIgnoreCase));

            var variantsArr = variants.ToArray();

            if (variantsArr.Length == 0)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                return null;
            }

            if (variantsArr.Length > 1)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.MultipleChoices;
#else
                Response.StatusCode = (int)HttpStatusCode.MultipleChoices;
#endif
                return null;
            }

            var variant = variantsArr[0];

            var isPreview = !string.IsNullOrEmpty(preview) && preview.Equals("true", StringComparison.OrdinalIgnoreCase);

            return GetFile(variant, isPreview ? FileType.Preview : FileType.File);
        }

        private Stream GetFile(string contentId, string variantName, FileType type)
        {
            var guid = ConvertGuid(contentId);
            if (guid == Guid.Empty)
                return null;

            var variant = ContentManager.GetVariant(guid, variantName);
            if (variant == null)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                return null;
            }

            return GetFile(variant, type);
        }

        private Stream GetFile(VariantDescriptor variantDescriptor, FileType type)
        {
            FileDescriptor descriptor;
            switch (type)
            {
                case FileType.File:
                    descriptor = variantDescriptor;
                    break;
                case FileType.Preview:
                    descriptor = variantDescriptor.Preview;
                    break;
                default:
#if USE_WCF
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                    return null;
            }

            var stream = ContentManager.GetStream(descriptor);
            if (stream == null)
            {
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
                return null;
            }


#if USE_WCF
            var context = WebOperationContext.Current;
            // ReSharper disable once PossibleNullReferenceException
            context.OutgoingResponse.ContentType = descriptor.MimeType;
            context.OutgoingResponse.ContentLength = descriptor.Size;
#else
            Response.ContentType = descriptor.MimeType;
            Response.ContentLength = descriptor.Size;
#endif

            return stream;
        }

        private Guid ConvertGuid(string stringGuid)
        {
            if (Guid.TryParse(stringGuid, out var guid))
                return guid;

#if USE_WCF
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
            Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
            return Guid.Empty;
        }

        private enum FileType
        {
            File,
            Preview
        }
    }
}
