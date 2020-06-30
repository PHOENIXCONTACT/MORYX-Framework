// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.Tools.Wcf;

namespace Moryx.DependentTestModule
{
    [ServiceContract]
    [ServiceVersion(ServerVersion = SimpleHelloWorldWcfService.ServerVersion, MinClientVersion = SimpleHelloWorldWcfService.MinClientVersion)]
    public interface ISimpleHelloWorldWcfService
    {
        [OperationContract(IsOneWay = false)]
        string Hello(string name);

        [OperationContract(IsOneWay = false)]
        string Throw(string name);
    }
}
