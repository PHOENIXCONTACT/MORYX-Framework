// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    public interface IHelloWorldWcfConnector : IWcfConnector<HelloWorldWcfConnectorConfig>
    {
        void TriggerHelloCallback(string name);
    }
}
