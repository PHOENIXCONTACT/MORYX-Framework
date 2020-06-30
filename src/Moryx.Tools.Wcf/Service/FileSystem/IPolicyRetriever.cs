// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Marvin.Tools.Wcf.FileSystem
{
    [ServiceContract]
    public interface IPolicyRetriever
    {
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
        Stream GetSilverlightPolicy();

        [OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
        Stream GetFlashPolicy();
    }
}
