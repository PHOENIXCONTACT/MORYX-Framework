// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer;
using Moryx.Configuration;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Simulator
{
    /// <summary>
    /// Configuration of this module
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        [DataMember, DefaultValue(50)]
        [DisplayName("Success rate")]
        [Description("Success rate for the simulation")]
        public int SuccessRate { get; set; }

        [DataMember, DefaultValue(3000)]
        [DisplayName("Transport duration")]
        [Description("Simulated time between two activities")]
        public int MovingDuration { get; set; }

        [DataMember, DefaultValue(2400)]
        [DisplayName("Default execution time")]
        [Description("Default activity execution time in MS. Instant result with -1")]
        public int DefaultExecutionTime { get; set; }

        [DataMember, DefaultValue(1)]
        [DisplayName("Acceleration")]
        [Description("Acceleration factor for the simulation.")]
        public double Acceleration { get; set; }

        [DataMember]
        [DisplayName("Specific execution times")]
        [Description("Activity and cell specific simulation times overriding the default configuration.")]
        public List<ExecutionTimeDefinition> SpecificExecutionTimeSettings { get; set; } = new List<ExecutionTimeDefinition>();   
    }

    public class ExecutionTimeDefinition
    {
        [DataMember, EntrySerialize]
        [PossibleTypes(typeof(Activity),UseFullname =true)]
        public string Activity { get; set; }

        [DataMember, EntrySerialize]
        [Description("Activity execution delay in MS. Instant result with -1")]
        public int ExecutionTime { get; set; }

        [DataMember, EntrySerialize]
        [Description("If the Cell ID is set to 0, this time is set for all resources, which don't have a specific entry")]
        public int CellId { get; set; }

        public override string ToString()
        {
            var cellString = CellId == 0 ? "" : CellId.ToString() + ": ";
            var activityString = Activity.Split('.')[^1];
            var timeString = ExecutionTime <= 0 ? "Instant" : ExecutionTime.ToString() +"ms";
            return $"{cellString}{activityString} - {timeString}";
        }
    }

}

