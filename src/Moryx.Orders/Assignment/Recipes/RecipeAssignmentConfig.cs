// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Base config for <see cref="IRecipeAssignment"/> implementations
    /// </summary>
    public class RecipeAssignmentConfig : IPluginConfig
    {
        /// <inheritdoc cref="IPluginConfig"/>
        [DataMember]
        [PluginNameSelector(typeof(IRecipeAssignment))]
        public virtual string PluginName { get; set; }
    }
}