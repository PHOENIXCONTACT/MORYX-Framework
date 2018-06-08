using Marvin.AbstractionLayer;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Plugin to fetch possible values from storage
    /// </summary>
    public interface ICustomization
    {
        /// <summary>
        /// Types of recipes avaible on this application
        /// </summary>
        string[] RecipeTypes { get; }

        /// <summary>
        /// Create prototype instance of this type
        /// </summary>
        IProductRecipe RecipePrototype(string type);

        /// <summary>
        /// Get possible value keys supported by storage
        /// </summary>
        string[] SupportedStorageKeys { get; }

        /// <summary>
        /// Fetch possbiel values from storage
        /// </summary>
        string[] AvailableStorageValues(string key);

        /// <summary>
        /// Add possible value to database for this keys
        /// </summary>
        void AddValue(string key, string value);

        /// <summary>
        /// Remove value for this keys
        /// </summary>
        /// <returns>False if value is still in use</returns>
        bool RemoveValue(string key, string value);
    }
}