// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.ProcessData.Listener
{
    /// <summary>
    /// Base class for process data listener configs
    /// </summary>
    [DataContract]
    public class ProcessDataListenerConfig : IPluginConfig
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProcessDataListenerConfig"/>
        /// </summary>
        public ProcessDataListenerConfig()
        {
            MeasurandConfigs = new List<MeasurandConfig>();
        }

        /// <inheritdoc />
        [DataMember, PluginNameSelector(typeof(IProcessDataListener))]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Unique Name of the Listener
        /// </summary>
        [DataMember]
        [Description("Unique Name of the Listener")]
        public string ListenerName { get; set; }

        /// <summary>
        /// Configuration for the occurred measurand. They can be enabled or disabled.
        /// </summary>
        [DataMember]
        public List<MeasurandConfig> MeasurandConfigs { get; set; }
    }

    /// <summary>
    /// Configuration for a measurand
    /// </summary>
    [DataContract]
    public class MeasurandConfig
    {
        /// <summary>
        /// Name of the measurand
        /// </summary>
        [DataMember]
        [Description("Name of the measurand")]
        public string Name { get; set; }

        /// <summary>
        /// Indicator if measurand processing is enabled
        /// </summary>
        [DataMember]
        [Description("If enabled, process data will be processed")]
        public bool IsEnabled { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var status = IsEnabled ? "Enabled" : "Disabled";
            return $"{Name}: {status}";
        }
    }
}