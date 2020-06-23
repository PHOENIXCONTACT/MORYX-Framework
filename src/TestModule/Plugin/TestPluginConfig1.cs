// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.TestModule
{
    public class TestPluginConfig1 : TestPluginConfig
    {
        public TestPluginConfig1()
        {
            OrderWcfService = new HostConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "OrderImporting",
                MetadataEnabled = true,
                HelpEnabled = true
            };

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
        /// Gets or sets the configuration for the wcf service.
        /// </summary>
        [DataMember]
        public HostConfig OrderWcfService { get; set; }

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
