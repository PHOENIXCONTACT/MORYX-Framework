// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.TestModule
{
    [DataContract]
    public class AnotherPluginConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        [DataMember]
        public string PluginName { get; set; }
    }

    [DataContract]
    public class AnotherPluginConfig2 : AnotherPluginConfig
    {
        [DataMember]
        [PluginConfigs(typeof(IAnotherSubPlugin))]
        public List<AnotherSubConfig> SubConfigs { get; set; }
    }

    [DataContract]
    public class AnotherSubConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        [DataMember]
        public string PluginName { get; set; }
    }

    [DataContract]
    public class AnotherSubConfig2 : AnotherSubConfig
    {
        [DataMember]
        public int Value { get; set; }
    }
}
