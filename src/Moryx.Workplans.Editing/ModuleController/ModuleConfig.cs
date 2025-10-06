// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.Workplans.Editing
{
    /// <summary>
    /// Configuration of <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Default serialization setting for tasks found in AppDomain
        /// </summary>
        [DataMember]
        [Description("Hide steps not configured in 'StepSettings'")]
        public bool HideUnknown { get; set; }

        /// <summary>
        /// Settings for steps
        /// </summary>
        [DataMember]
        public List<StepConfig> StepSettings { get; set; }
    }

    /// <summary>
    /// Config for a single step
    /// </summary>
    [DataContract]
    public class StepConfig
    {
        /// <summary>
        /// Type of step represented by this entry
        /// </summary>
        [DataMember]
        [PossibleTypes(typeof(IWorkplanStep), UseFullname = true)]
        public string StepType { get; set; }

        /// <summary>
        /// Visibility setting
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }
    }
}
