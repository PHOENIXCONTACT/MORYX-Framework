// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.ProcessData.Listener;

namespace Moryx.ProcessData.SpreadsheetsListener
{
    /// <summary>
    /// Configuration for the <see cref="ProcessData.SpreadsheetsListener"/>
    /// </summary>
    [DataContract]
    public class SpreadsheetsListenerConfig : ProcessDataListenerConfig
    {
        /// <inheritdoc />
        public override string PluginName
        {
            get => nameof(SpreadsheetsListener);
            set { }
        }

        /// <summary>
        /// Interval in milliseconds for the report
        /// </summary>
        [DataMember, DefaultValue(10000)]
        [Description("Interval in milliseconds for the report")]
        public int ReportIntervalMs { get; set; }

        /// <summary>
        /// Interval in milliseconds for the report
        /// </summary>
        [DataMember, DefaultValue(10000)]
        [Description("Maximum number of rows including header")]
        public int MaxNumOfRows { get; set; }

        /// <summary>
        /// Path where CSV files are stored
        /// </summary>
        [DataMember, DefaultValue("C:\\MORYX\\ProcessDataMonitor\\csv\\")]
        [Description("Path where CSV files are stored")]
        public string Path { get; set; }

        /// <summary>
        /// Delimiter used to separate values
        /// </summary>
        [DataMember, DefaultValue(";")]
        [Description("Delimiter used to separate values")]
        public string Delimiter { get; set; }
    }
}
