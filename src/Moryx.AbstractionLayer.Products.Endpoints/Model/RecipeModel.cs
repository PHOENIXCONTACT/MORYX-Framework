// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// DTO representation of a recipe
    /// </summary>
    [DataContract]
    public class RecipeModel
    {
        /// <summary>
        /// Id of the recipe
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Name of the recipe
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// This recipe's state
        /// </summary>
        [DataMember]
        public RecipeState State { get; set; }

        /// <summary>
        /// Revision of the recipe
        /// </summary>
        [DataMember]
        public int Revision { get; set; }

        /// <summary>
        /// Type of the recipe
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Properties of the recipe
        /// </summary>
        [DataMember]
        public Entry Properties { get; set; }

        /// <summary>
        /// The id of the currently referenced workplan
        /// </summary>
        [Obsolete("Use WorkplanModel instead")]
        [DataMember]
        public long WorkplanId { get; set; }

        /// <summary>
        /// A model of the currently referenced workplan
        /// </summary>
        [DataMember]
        public WorkplanModel WorkplanModel { get; set; }

        /// <summary>
        /// Classification of the recipe
        /// </summary>
        [DataMember]
        public RecipeClassificationModel Classification { get; set; }

        /// <summary>
        /// Whether this Recipe is a clone or not
        /// </summary>
        [DataMember]
        public bool IsClone { get; set; }
    }
}
