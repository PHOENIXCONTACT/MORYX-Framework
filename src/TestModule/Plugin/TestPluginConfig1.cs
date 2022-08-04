// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.TestModule
{
    public class TestPluginConfig1 : TestPluginConfig
    {
        public TestPluginConfig1()
        {
            OrderSources = new List<SourceConfig>();
        }

        /// <summary>
        /// The name of the component.
        /// </summary>
        [DataMember]
        [PluginNameSelector(typeof(ITestPlugin))]
        [DefaultValue(TestPlugin1.ComponentName)]
        public override string PluginName { get; set; }

        /// <summary>
        /// Gets or sets the order source.
        /// </summary>
        /// <value>
        /// The order source.
        /// </value>
        [DataMember]
        public List<SourceConfig> OrderSources { get; set; }
    }
}
