// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.Communication.Endpoints;

namespace Moryx.DependentTestModule
{
    [ServiceContract]
    [Endpoint(Name = nameof(ISimpleHelloWorldWcfService), Version = SimpleHelloWorldWcfService.ServerVersion)]
    public interface ISimpleHelloWorldWcfService
    {
        [OperationContract(IsOneWay = false)]
        string Hello(string name);

        [OperationContract(IsOneWay = false)]
        string Throw(string name);
    }
}
