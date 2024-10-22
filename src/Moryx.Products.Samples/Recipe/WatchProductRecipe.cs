// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Serialization;

namespace Moryx.Products.Samples.Recipe
{
    public class WatchProductRecipe : ProductionRecipe
    {
        public WatchProductRecipe()
        {
        }

        public WatchProductRecipe(WatchProductRecipe source) : base(source)
        {
            CoresInstalled = source.CoresInstalled;

            Case = source.Case;
        }

        [EntrySerialize, FacacadeRecipeValue]
        [DisplayName("Cores Installed")]
        public int CoresInstalled { get; set; }

        [EntrySerialize]
        [DisplayName("Case Parameters")]
        public CaseDescription Case { get; set; }

        /// <inheritdoc />
        public override IRecipe Clone()
        {
            return new WatchProductRecipe(this);
        }

        /// <summary>
        /// Create a <see cref="ProductionProcess"/> for this recipe
        /// </summary>
        public override IProcess CreateProcess() =>
            new ProductionProcess { Recipe = this };
    }

    public class CaseDescription
    {
        [EntrySerialize]
        [DisplayName("Color Code")]
        [Description("Numeric code of the color")]
        public int CaseColorCode { get; set; }

        [EntrySerialize]
        [DisplayName("Material")]
        public int CaseMaterial { get; set; }
    }
}
