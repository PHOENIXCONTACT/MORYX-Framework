// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using Moryx.Communication.Endpoints;
using Moryx.Tools.Wcf;

namespace Moryx.TestModule
{
    [Endpoint(Name = nameof(IHelloWorldWcfService), Version = HelloWorldWcfService.ServerVersion)]
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IHelloWorldWcfServiceCallback))]
    public interface IHelloWorldWcfService : ISessionService
    {
        [OperationContract(IsOneWay = false)]
        string Hello(string name);

        [OperationContract(IsOneWay = false)]
        string Throw(string name);

        [OperationContract(IsOneWay = true)]
        void TriggerHelloCallback(string name);

        [OperationContract(IsOneWay = true)]
        void TriggerThrowCallback(string name);

        void HelloCallback(string message);

        string ThrowCallback(string message);

        [OperationContract(IsOneWay = true)]
        void DeferredDisconnect(int waitInMs);
    }
}
