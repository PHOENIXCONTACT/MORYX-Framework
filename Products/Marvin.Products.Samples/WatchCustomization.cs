using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Products.Management;

namespace Marvin.Products.Samples
{
    [Plugin(LifeCycle.Singleton, typeof(ICustomization))]
    public class WatchCustomization : ICustomization
    {
       /// <summary>
        /// Types of recipes avaible on this application
        /// </summary>
        public string[] RecipeTypes { get { return new[] { "TestRecipe" }; } }

        /// <summary>
        /// Create prototype instance of this type
        /// </summary>
        public IProductRecipe RecipePrototype(string type)
        {
            return new ProductRecipe();
        }

        /// <summary>
        /// Get possible value keys supported by storage
        /// </summary>
        public string[] SupportedStorageKeys
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// Fetch possbiel values from storage
        /// </summary>
        public string[] AvailableStorageValues(string key)
        {
            return new string[] { };
        }

        /// <summary>
        /// Add possible value to database for this keys
        /// </summary>
        public void AddValue(string key, string value)
        {
        }

        /// <summary>
        /// Remove value for this keys
        /// </summary>
        /// <returns>False if value is still in use</returns>
        public bool RemoveValue(string key, string value)
        {
            return false;
        }
    }
}