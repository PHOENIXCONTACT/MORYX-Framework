// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;

namespace Moryx.Container.Tests
{
    [Plugin(LifeCycle.Singleton, typeof(IRootClass), Name = PluginName)]
    public class RootClass : IRootClass
    {
        internal const string PluginName = "RootClass";

        // Injected
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

        /// <inheritdoc />
        public void Stop()
        {
        }
    }

    [Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
    public class ConfiguredComponentA : IConfiguredComponent
    {
        internal const string PluginName = "ConfiguredA";

        public string GetName()
        {
            return PluginName;
        }

        /// <inheritdoc />
        public void Initialize(ComponentConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }
    }

    [Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
    public class ConfiguredComponentB : IConfiguredComponent
    {
        internal const string PluginName = "ConfiguredB";

        public string GetName()
        {
            return PluginName;
        }

        /// <inheritdoc />
        public void Initialize(ComponentConfig config)
        {
        }

        /// <inheritdoc />
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
        }
    }
}
