// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;

namespace Moryx.Container.TestRootPlugin
{
    [DependencyRegistration(InstallerMode.All)]
    [Plugin(LifeCycle.Singleton, typeof(IRootClass), Name = PluginName)]
    public class ForeignRootClass : IRootClass
    {
        public const string PluginName = "ForeignRootClass";

        public IConfiguredComponent ConfiguredComponent { get; set; }

        public string GetName()
        {
            return PluginName;
        }

        public void Initialize(RootClassFactoryConfig config)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
