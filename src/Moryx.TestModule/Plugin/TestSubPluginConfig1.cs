// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.TestModule
{
    [DataContract]
    public class TestSubPluginConfig1 : TestSubPluginConfig
    {
        public TestSubPluginConfig1()
        {
            OrderSources = [];
        }

        public override string PluginName { get { return TestSubPlugin1.ComponentName; } }

        /// <summary>
        /// Gets or sets the order source.
        /// </summary>
        /// <value>
        /// The order source.
        /// </value>
        [DataMember]
        public List<SourceConfig> OrderSources { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SourceConfig
    {
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        [DataMember]
        public int SourceId { get; set; }

        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        [DataMember]
        public string SourceName { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Source: '" + (!string.IsNullOrWhiteSpace(SourceName) ? SourceName : string.Empty) + "' ID: '" + SourceId + "'";
        }
    }
}
