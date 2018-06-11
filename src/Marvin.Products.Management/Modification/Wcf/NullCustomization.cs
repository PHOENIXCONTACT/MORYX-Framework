using System;
using Marvin.AbstractionLayer;

namespace Marvin.Products.Management.Modification
{
    /// <summary>
    /// Null class for the customization to have the posibility to use the abstraction layer without a custom customization in case a recipe is not necessary
    /// </summary>
    internal class NullCustomization : ICustomization
    {
        public string[] RecipeTypes { get { return new string[0]; } }

        public string[] SupportedStorageKeys { get { return new string[0]; } }

        public IProductRecipe RecipePrototype(string type)
        {
            throw new NotImplementedException();
        }
        public string[] AvailableStorageValues(string key)
        {
            throw new NotImplementedException();
        }
        public void AddValue(string key, string value)
        {
            throw new NotImplementedException();
        }
        public bool RemoveValue(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}