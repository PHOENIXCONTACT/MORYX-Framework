// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Modules;

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Configuration of a scheduler
    /// </summary>
    [DataContract]
    public abstract class JobSchedulerConfig : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public abstract string PluginName { get; }
    }
}
