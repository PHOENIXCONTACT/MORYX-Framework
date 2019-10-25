using System;
using Marvin.AbstractionLayer;
using Marvin.Modules;
using Marvin.Products.Model;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Interface for plugins that can convert recipes
    /// </summary>
    public interface IProductRecipeStrategy : IConfiguredPlugin<ProductRecipeConfiguration>
    {
        /// <summary>
        /// Target type of this strategy
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Write recipe properties to database generic columns
        /// </summary>
        void SaveRecipe(IProductRecipe source, IGenericColumns target);

        /// <summary>
        /// Load recipe from database information
        /// </summary>
        void LoadRecipe(IGenericColumns source, IProductRecipe target);
    }
}