// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.ControlSystem.Jobs;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Config for the parallel scheduler
    /// </summary>
    [DataContract]
    internal class ParallelSchedulerConfig : JobSchedulerConfig
    {
        /// <inheritdoc />
        public override string PluginName => nameof(ParallelScheduler);

        /// <summary>
        /// Gets or sets the maximum amount of active jobs when LimitActiveJobs = true. 
        /// </summary>
        [DataMember, DefaultValue(1)]
        [Description("Maximum amount of active jobs!")]
        public ushort MaxActiveJobs { get; set; }
    }
}
