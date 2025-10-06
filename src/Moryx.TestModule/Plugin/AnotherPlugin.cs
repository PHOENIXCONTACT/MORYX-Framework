// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.TestModule
{
    [Plugin(LifeCycle.Singleton, typeof(IAnotherPlugin))]
    public class AnotherPlugin : IAnotherPlugin
    {
        /// <inheritdoc />
        public void Initialize(AnotherPluginConfig config)
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

    [ExpectedConfig(typeof(AnotherPluginConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(IAnotherPlugin))]
    public class AnotherPlugin2 : IAnotherPlugin
    {
        /// <inheritdoc />
        public void Initialize(AnotherPluginConfig config)
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

    [Plugin(LifeCycle.Singleton, typeof(IAnotherSubPlugin))]
    public class AnotherSubPlugin : IAnotherSubPlugin
    {
        /// <inheritdoc />
        public void Initialize(AnotherSubConfig config)
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

    [ExpectedConfig(typeof(AnotherSubConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(IAnotherSubPlugin))]
    public class AnotherSubPlugin2 : IAnotherSubPlugin
    {
        /// <inheritdoc />
        public void Initialize(AnotherSubConfig config)
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
