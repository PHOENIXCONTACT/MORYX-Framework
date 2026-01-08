// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Config for the seamless scheduler strategy
    /// </summary>
    internal class SeamlessSchedulerConfig : JobSchedulerConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(SeamlessScheduler);
    }
}
