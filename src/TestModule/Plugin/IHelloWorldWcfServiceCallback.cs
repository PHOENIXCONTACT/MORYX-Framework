// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;

namespace Moryx.TestModule
{
    public interface IHelloWorldWcfServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void HelloCallback(string message);

        [OperationContract(IsOneWay = false)]
        string ThrowCallback(string message);

    }
}
