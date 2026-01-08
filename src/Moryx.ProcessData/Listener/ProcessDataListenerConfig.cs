// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.ProcessData.Properties;
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
        [Display(Name = nameof(Strings.ProcessDataListenerConfig_ListenerName), Description = nameof(Strings.ProcessDataListenerConfig_ListenerName_Description), ResourceType = typeof(Strings))]
        public string ListenerName { get; set; }

        /// <summary>
        /// Configuration for the occurred measurand. They can be enabled or disabled.
        /// </summary>
        [DataMember]
        [Display(Name = nameof(Strings.ProcessDataListenerConfig_MeasurandConfigs), ResourceType = typeof(Strings))]
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
        [Display(Name = nameof(Strings.MeasurandConfig_Name), Description = nameof(Strings.MeasurandConfig_Name_Description), ResourceType = typeof(Strings))]
        public string Name { get; set; }

        /// <summary>
        /// Indicator if measurand processing is enabled
        /// </summary>
        [DataMember]
        [Display(Name = nameof(Strings.MeasurandConfig_IsEnabled), Description = nameof(Strings.MeasurandConfig_IsEnabled_Description), ResourceType = typeof(Strings))]
        public bool IsEnabled { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var status = IsEnabled ? "Enabled" : "Disabled";
            return $"{Name}: {status}";
        }
    }
}
