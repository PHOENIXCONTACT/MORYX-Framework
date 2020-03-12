// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Serialization;

namespace Marvin.DependentTestModule
{
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        [DataMember]
        [PluginConfigs(typeof(ISimpleHelloWorldWcfConnector))]
        public SimpleHelloWorldWcfConnectorConfig SimpleHelloWorldWcfConnector { get; set; }
    }
}
