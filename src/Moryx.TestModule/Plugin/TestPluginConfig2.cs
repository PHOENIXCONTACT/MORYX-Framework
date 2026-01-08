// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Serialization;

namespace Moryx.TestModule
{
    public class TestPluginConfig2 : TestPluginConfig
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        [DataMember]
        [PluginNameSelector(typeof(ITestPlugin))]
        [DefaultValue(TestPlugin2.ComponentName)]
        public override string PluginName { get; set; }
    }
}
