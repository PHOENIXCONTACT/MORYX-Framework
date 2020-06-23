// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.DependentTestModule
{
    [DataContract]
    public class SimpleHelloWorldWcfConnectorConfig : BasicWcfConnectorConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHelloWorldWcfConnectorConfig"/> class.
        /// </summary>
        public SimpleHelloWorldWcfConnectorConfig() : base(SimpleHelloWorldWcfService.ServiceName)
        {
            ConnectorHost.BindingType = ServiceBindingType.BasicHttp;
        }

        public override string PluginName => nameof(SimpleHelloWorldWcfConnector);
    }
}
