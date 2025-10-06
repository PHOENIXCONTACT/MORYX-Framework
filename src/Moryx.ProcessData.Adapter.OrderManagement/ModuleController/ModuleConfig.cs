// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.ProcessData.Adapter.OrderManagement
{
    /// <summary>
    /// Module configuration of the adapter <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <summary>
        /// Interval for reporting progress changes
        /// </summary>
        [DataMember, Description("Interval for reporting progress changes")]
        [DefaultValue(10000)]
        public int ReportInterval { get; set; }
    }
}
