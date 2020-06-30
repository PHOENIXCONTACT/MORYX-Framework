// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools.Wcf;

namespace Moryx.DependentTestModule
{
    public interface ISimpleHelloWorldWcfConnector : IWcfConnector<SimpleHelloWorldWcfConnectorConfig>
    {
    }
}
