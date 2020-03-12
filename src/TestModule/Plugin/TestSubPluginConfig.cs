// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Marvin.Modules;

namespace Marvin.TestModule
{
    [DataContract]
    public class TestSubPluginConfig : IPluginConfig
    {
        public virtual string PluginName { get { return TestSubPlugin.ComponentName; } }
    }
}
