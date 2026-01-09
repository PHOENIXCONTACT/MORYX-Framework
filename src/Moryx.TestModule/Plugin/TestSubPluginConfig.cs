// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;

namespace Moryx.TestModule;

[DataContract]
public class TestSubPluginConfig : IPluginConfig
{
    public virtual string PluginName { get { return TestSubPlugin.ComponentName; } }
}