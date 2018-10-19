using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [Plugin(LifeCycle.Transient, typeof(IProductInteraction))]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    internal class ProductInteraction : IProductInteraction
    {
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public ICustomization Customization { get; set; }

        public IProductConverterFactory ConverterFactory { get; set; }

        public IProductManager Manager { get; set; }

        #endregion

        public ProductCustomization GetCustomization()
        {
            // Use customization if avialable or its NullStrategy
            var customization = Customization;

            var storageValues = new List<StorageValue>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var storageKey in customization.SupportedStorageKeys)
            {
                var values = customization.AvailableStorageValues(storageKey);
                storageValues.Add(new StorageValue
                {
                    Key = storageKey,
                    Values = values
                });
            }

            return new ProductCustomization
            {
                ReleasedProductsEditable = Config.ReleasedProductEditable,
                HasRecipes = Config.HasRecipes,
                RecipeTypes = customization.RecipeTypes,
                StorageValues = storageValues.ToArray(),
                Importers = Manager.Importers.Select(i => new ProductImporter
                {
                    Name = i.Name,
                    Parameters = ConvertParameters(i.Parameters)
                }).ToArray()
            };
        }

        private static Entry ConvertParameters(IImportParameters parametersObject) => 
            EntryConvert.EncodeObject(parametersObject, new PartialSerialization<IImportParameters>());

        public Entry UpdateParameters(string importerName, Entry currentParameters)
        {
            var parameters = ConvertParametersBack(importerName, currentParameters, true);
            return ConvertParameters(parameters);
        }

        private IImportParameters ConvertParametersBack(string importerName, Entry currentParameters, bool updateFirst = false)
        {
            var importer = Manager.Importers.First(i => i.Name == importerName);
            var parameters = (IImportParameters)EntryConvert.UpdateInstance(importer.Parameters, currentParameters);
            if (updateFirst)
                parameters = importer.Update(parameters);
            return parameters;
        }

        public ProductStructureEntry[] GetProductStructure()
        {
            return Manager.ExportTree();
        }

        public ProductModel[] GetAllProducts()
        {
            return UseConverter(c => c.GetRootProducts());
        }

        public ProductModel GetProductDetails(long id)
        {
            return UseConverter(c => c.GetProduct(id));
        }

        public ProductRevisionEntry[] GetProductRevisions(string identifier)
        {
            return Manager.Revisions(identifier);
        }

        public ProductModel SaveProduct(ProductModel instance)
        {
            return UseConverter(c => c.Save(instance));
        }

        public ProductModel ReleaseProduct(long id)
        {
            return UseConverter(c => c.ReleaseProduct(id));
        }

        public ProductModel CreateRevision(long id, short revisionNo, string comment)
        {
            return UseConverter(c => c.CreateRevision(id, revisionNo, comment));
        }

        public ProductModel ImportProduct(string importerName, Entry parametersModel)
        {
            var parameters = ConvertParametersBack(importerName, parametersModel);
            return UseConverter(c => c.ImportProduct(importerName, parameters));
        }

        public ProductModel[] DeleteProduct(long id)
        {
            return UseConverter(c => c.DeleteProduct(id));
        }

        public RecipeModel GetRecipe(long recipeId)
        {
            return UseConverter(c => c.GetRecipe(recipeId));
        }

        public RecipeModel CreateRecipe(string recipeType)
        {
            return UseConverter(c => c.CreateRecipe(recipeType));
        }

        public RecipeModel GetProductionRecipe(long productId, long workplanId)
        {
            return UseConverter(c => c.GetProductionRecipe(productId, workplanId));
        }

        public RecipeModel CreateProductionRecipe(long productId, long workplanId, string name)
        {
            return UseConverter(c => c.CreateProductionRecipe(productId, workplanId, name));
        }

        public string GetRecipeProviderName()
        {
            return ModuleController.ModuleName;
        }

        private TResult UseConverter<TResult>(Func<IProductConverter, TResult> call)
        {
            var converter = ConverterFactory.Create();
            var converted = call(converter);
            ConverterFactory.Destroy(converter);
            return converted;
        }
    }
}