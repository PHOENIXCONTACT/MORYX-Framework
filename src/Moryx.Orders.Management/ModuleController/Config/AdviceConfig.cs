// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Orders.Advice;
using Moryx.Runtime.Configuration;
using Moryx.Serialization;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Configuration for the advice manager
    /// </summary>
    [DataContract]
    public class AdviceConfig
    {
        /// <inheritdoc cref="IAdviceExecutor"/>
        [DataMember, Description("Configured executor for advices")]
        [ModuleStrategy(typeof(IAdviceExecutor))]
        [PluginConfigs(typeof(IAdviceExecutor))]
        public AdviceExecutorConfig AdviceExecutor { get; set; }

        /// <summary>
        /// Indicates if the configured executor should be used for order advices
        /// </summary>
        [DataMember, Description("Indicates if the configured executor should be used for order advices")]
        public bool UseAdviceExecutorForOrderAdvice { get; set; }
    }
}
