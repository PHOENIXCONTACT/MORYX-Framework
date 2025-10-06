// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#if USE_WCF
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Communication.Endpoints;
using Moryx.Web;
#endif

namespace Moryx.Media.Server.Endpoint
{
#if USE_WCF
    [ServiceContract]
    [Endpoint(Name = nameof(IMediaService), Version = "4.0.0")]

#endif
    internal interface IMediaService
    {
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors", Method = WebRequestMethods.Http.Get, ResponseFormat = WebMessageFormat.Json)]
#endif
        ContentDescriptorModel[] GetDescriptors();

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors/query?start={start}&offset={offset}", Method = WebRequestMethods.Http.Get, ResponseFormat = WebMessageFormat.Json)]
#endif
        ContentDescriptorModel[] GetDescriptorsByPage(uint start, uint offset);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors/{contentId}", Method = WebRequestMethods.Http.Get, ResponseFormat = WebMessageFormat.Json)]
#endif
        ContentDescriptorModel GetDescriptor(string contentId);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors", Method = WebRequestMethods.Http.Post, ResponseFormat = WebMessageFormat.Json)]
#endif
        Task<ContentAddingInfoModel> AddContent(ContentUploadRequest uploadRequest);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors/{contentId}/{variantName}", Method = WebRequestMethodsExtension.Http.Delete, ResponseFormat = WebMessageFormat.Json)]
#endif
        bool RemoveVariant(string contentId, string variantName);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors/{contentId}/{variantName}", Method = WebRequestMethods.Http.Get, ResponseFormat = WebMessageFormat.Json)]
#endif
        VariantDescriptorModel GetVariant(string contentId, string variantName);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "descriptors/{contentId}/{variantName}/preview", Method = WebRequestMethods.Http.Get, ResponseFormat = WebMessageFormat.Json)]
#endif
        PreviewDescriptorModel GetPreview(string contentId, string variantName);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = InternalConstants.FileUriTemplate, Method = WebRequestMethods.Http.Get)]
#endif
        Stream DownloadVariant(string contentId, string variantName);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = InternalConstants.PreviewUriTemplate, Method = WebRequestMethods.Http.Get)]
#endif
        Stream DownloadPreview(string contentId, string variantName);

#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "files?fileName={fileName}&variant={variantName}&fileHash={fileHash}&preview={preview}", Method = WebRequestMethods.Http.Get)]
#endif
        Stream FileSearch(string fileHash = null, string fileName = null, string variantName = MediaConstants.MasterName, string preview = "false");
    }
}
