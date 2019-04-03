﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;
using Marvin.Modules;
using Marvin.Serialization;

namespace Marvin.TestModule
{
    public class TestPluginConfig : IPluginConfig
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        [DataMember]
        [PluginNameSelector(typeof(ITestPlugin))]
        [DefaultValue(TestPlugin.ComponentName)]
        public virtual string PluginName { get; set; }

        [DataMember]
        [DefaultValue(500)]
        public int PluginIntegerValue { get; set; }

        [DataMember]
        public bool PluginBoolValue { get; set; }

        [DataMember]
        [DefaultValue(420000000000000)]
        public long PluginLongValue { get; set; }

        [DataMember]
        [DefaultValue("Hallo")]
        public string PluginStringValue { get; set; }

        [DataMember]
        [DefaultValue(3.14159)]
        public double PluginDoubleValue { get; set; }

        [DataMember]
        [DefaultValue(ConfigEnumeration.Value9)]
        public ConfigEnumeration PluginEnumValue { get; set; }

        [DataMember]
        [PluginConfigs(typeof(ITestSubPlugin))]
        public TestSubPluginConfig SubPlugin { get; set; }

        [DataMember]
        [PluginConfigs(typeof(ITestSubPlugin), false)]
        public List<TestSubPluginConfig> SubPlugins { get; set; }
         
    }
}