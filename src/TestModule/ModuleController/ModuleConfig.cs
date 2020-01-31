// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Logging;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.TestModule
{
    public enum ConfigEnumeration
    {
        Value0,
        Value1,
        Value2,
        Value3,
        Value4,
        Value5,
        Value6,
        Value7,
        Value8,
        Value9
    }

    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        [SharedConfig(false)]
        public PortConfig Config { get; set; }

        [DataMember]
        [DefaultValue(0x2)]
        public byte ByteValue { get; set; }

        [DataMember]
        [DefaultValue(5)]
        public int IntegerValue { get; set; }

        [DataMember]
        public bool BoolValue { get; set; }

        [DataMember]
        [DefaultValue(4200000000000)]
        public long LongValue { get; set; }

        [DataMember]
        [DefaultValue("Hello")]
        public string StringValue { get; set; }

        [DataMember]
        [DefaultValue(3.14)]
        public double DoubleValue { get; set; }

        [DataMember]
        [DefaultValue(ConfigEnumeration.Value0)]
        public ConfigEnumeration EnumValue { get; set; }

        [DataMember]
        [PluginConfigs(typeof(ITestPlugin))]
        public TestPluginConfig TestPlugin { get; set; }

        [DataMember]
        [PluginConfigs(typeof(IAnotherPlugin))]
        public AnotherPluginConfig AnotherPlugin { get; set; }

        [DataMember]
        [PluginConfigs(typeof(ITestPlugin), false)]
        public List<TestPluginConfig> Plugins { get; set; }

        [DataMember]
        [DefaultValue(LogLevel.Trace)]
        public LogLevel LogLevel { get; set; }

        [DataMember]
        [DefaultValue(2000)]
        public int SleepTime { get; set; }

        [DataMember]
        [PluginConfigs(typeof(IHelloWorldWcfConnector))]
        public HelloWorldWcfConnectorConfig HelloWorldWcfConnector { get; set; }
    }
}
