// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.Collections;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Configuration for implementations of
    /// </summary>
    public class CellSelectorConfig : IPluginConfig, ISortableObject
    {
        /// <inheritdoc />
        [DataMember, PluginNameSelector(typeof(ICellSelector))]
        public virtual string PluginName { get; set; }

        /// <summary>
        /// Order of execution for resource sorters
        /// </summary>
        [DataMember]
        [Description("Order of execution for all resource selectors")]
        public int SortOrder { get; set; }
    }
}
