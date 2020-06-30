// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Serialization;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Common configuration interface for all strategy configs
    /// </summary>
    public interface IProductStrategyConfiguation : IPluginConfig
    {
        /// <summary>
        /// Target type of the strategy
        /// </summary>
        string TargetType { get; set; }

        /// <summary>
        /// Changeable plugin name of the strategy
        /// </summary>
        new string PluginName { get; set; }
    }

    [DataContract]
    public class ProductTypeConfiguration : IProductStrategyConfiguation
    {
        [DataMember, PossibleTypes(typeof(ProductType))]
        public string TargetType { get; set; }

        [DataMember, PluginNameSelector(typeof(IProductTypeStrategy))]
        public virtual string PluginName { get; set; }

        public override string ToString()
        {
            return $"{TargetType} => {PluginName}";
        }
    }

    [DataContract]
    public class ProductInstanceConfiguration : IProductStrategyConfiguation
    {
        [DataMember, PossibleTypes(typeof(ProductInstance))]
        public string TargetType { get; set; }

        [DataMember, PluginNameSelector(typeof(IProductInstanceStrategy))]
        public virtual string PluginName { get; set; }

        public override string ToString()
        {
            return $"{TargetType}=>{PluginName}";
        }
    }

    [DataContract]
    public class ProductLinkConfiguration : IProductStrategyConfiguation
    {
        [DataMember, PossibleTypes(typeof(ProductType))]
        public string TargetType { get; set; }

        [DataMember]
        public string PartName { get; set; }

        [DataMember, PluginNameSelector(typeof(IProductLinkStrategy))]
        public virtual string PluginName { get; set; }

        [DataMember]
        public PartSourceStrategy PartCreation { get; }

        public override string ToString()
        {
            return $"{TargetType}.{PartName} => {PluginName}";
        }
    }

    [DataContract]
    public class ProductRecipeConfiguration : IProductStrategyConfiguation
    {
        [DataMember, PossibleTypes(typeof(IProductRecipe))]
        public string TargetType { get; set; }

        [DataMember, PluginNameSelector(typeof(IProductRecipeStrategy))]
        public virtual string PluginName { get; set; }

        public override string ToString()
        {
            return $"{TargetType} => {PluginName}";
        }
    }
}
