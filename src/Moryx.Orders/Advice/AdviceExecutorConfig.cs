// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Advice
{
    /// <summary>
    /// Base config for advice executors
    /// </summary>
    public class AdviceExecutorConfig : IPluginConfig
    {
        /// <inheritdoc />
        [DataMember, Description("PluginName of the advice executor")]
        [PluginNameSelector(typeof(IAdviceExecutor))]
        public virtual string PluginName { get; set; }
    }
}