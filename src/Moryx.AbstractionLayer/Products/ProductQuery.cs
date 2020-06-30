// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Products
{
    /// <summary>
    /// Filter to fetch products from the product management
    /// </summary>
    [DataContract]
    public class ProductQuery
    {
        /// <summary>
        /// Flag if deleted products should be included in the query
        /// </summary>
        [DataMember]
        public bool IncludeDeleted { get; set; }

        /// <summary>
        /// Product types to filter by
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// When filtering by type exclude derived types and only return specific matches
        /// </summary>
        [DataMember]
        public bool ExcludeDerivedTypes { get; set; }

        /// <summary>
        /// Material identifiers
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }

        /// <summary>
        /// Revisions to filter
        /// </summary>
        [DataMember]
        public RevisionFilter RevisionFilter { get; set; }

        /// <summary>
        /// Material identifiers
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Optional specific revision if <see cref="RevisionFilter"/> is set to
        /// <see cref="RevisionFilter.Specific"/>
        /// </summary>
        [DataMember]
        public short Revision { get; set; }

        /// <summary>
        /// Optional recipe filter
        /// </summary>
        [DataMember]
        public RecipeFilter RecipeFilter { get; set; }

        /// <summary>
        /// Selector
        /// </summary>
        [DataMember]
        public Selector Selector { get; set; }
    }

    /// <summary>
    /// Filter for revisions
    /// </summary>
    public enum RevisionFilter
    {
        /// <summary>
        /// Fetch all revisions, this is the default
        /// </summary>
        All = 0,
        /// <summary>
        /// Fetch only the latest revision
        /// </summary>
        Latest = 1,
        /// <summary>
        /// Fetch only specific revisions
        /// </summary>
        Specific = 2
    }

    /// <summary>
    /// Different options regarding the recipes of the product
    /// </summary>
    public enum RecipeFilter
    {
        /// <summary>
        /// Recipe information is not relevant
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Product has recipes
        /// </summary>
        WithRecipe = 1,

        /// <summary>
        /// Product does not have recipes
        /// </summary>
        WithoutRecipes = 2
    }

    /// <summary>
    /// Determine which objects to select for the query
    /// </summary>
    public enum Selector
    {
        /// <summary>
        /// Fetch the instances that match the query
        /// </summary>
        Direct,

        /// <summary>
        /// Fetch parents of instances that match the query
        /// </summary>
        Parent,

        /// <summary>
        /// Fetch parts of instances that match the query
        /// </summary>
        Parts,

    }
}
