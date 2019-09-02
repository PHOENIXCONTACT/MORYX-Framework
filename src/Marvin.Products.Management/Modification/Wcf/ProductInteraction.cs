using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Serialization;
using Marvin.Tools;

namespace Marvin.Products.Management.Modification
{
    [Plugin(LifeCycle.Transient, typeof(IProductInteraction))]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    internal class ProductInteraction : IProductInteraction
    {
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public IProductConverterFactory ConverterFactory { get; set; }

        public IProductManager Manager { get; set; }

        #endregion

        public ProductCustomization GetCustomization()
        {
            return new ProductCustomization
            {
                ProductTypes = ReflectionTool.GetPublicClasses<Product>()
                    // TODO: Replace with GetDisplayName in Platform3
                    .Select(pt => new ProductTypeModel
                    {
                        Name = pt.Name,
                        DisplayName = pt.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? pt.Name,
                        BaseType = pt.BaseType?.Name
                    }).ToArray(),
                RecipeTypes = ReflectionTool.GetPublicClasses<IProductRecipe>(t => Config.SupportedRecipes.Any(sp => t.Name == sp.Type))
                    // TODO: Replace with GetDisplayName in Platform3
                    .Select(rt => new RecipeTypeModel
                    {
                        Name = rt.Name,
                        DisplayName = rt.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? rt.Name,
                        HasWorkplans = typeof(IWorkplanRecipe).IsAssignableFrom(rt)
                    }).ToArray(),
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

        public ProductModel[] GetProducts(ProductQuery query)
        {
            return UseConverter(c => c.GetProducts(query));
        }

        public ProductModel CreateProduct(string type)
        {
            return UseConverter(c => c.Create(type));
        }

        public ProductModel GetProductDetails(long id)
        {
            return UseConverter(c => c.GetProduct(id));
        }

        public ProductModel SaveProduct(ProductModel instance)
        {
            return UseConverter(c => c.Save(instance));
        }

        public DuplicateProductResponse DuplicateProduct(long sourceId, string identifier, short revisionNo)
        {
            return UseConverter(c => c.Duplicate(sourceId, identifier, revisionNo));
        }

        public ProductModel ImportProduct(string importerName, Entry parametersModel)
        {
            var parameters = ConvertParametersBack(importerName, parametersModel);
            return UseConverter(c => c.ImportProduct(importerName, parameters));
        }

        public bool DeleteProduct(long id)
        {
            return UseConverter(c => c.DeleteProduct(id));
        }

        public string GetRecipeProviderName()
        {
            return ModuleController.ModuleName;
        }

        public RecipeModel GetRecipe(long recipeId)
        {
            return UseConverter(c => c.GetRecipe(recipeId));
        }

        public RecipeModel[] GetRecipes(long productId)
        {
            return UseConverter(c => c.GetRecipes(productId));
        }

        public RecipeModel CreateRecipe(string recipeType)
        {
            return UseConverter(c => c.CreateRecipe(recipeType));
        }

        public RecipeModel SaveRecipe(RecipeModel recipe)
        {
            return UseConverter(c => c.SaveRecipe(recipe));
        }

        public WorkplanModel[] GetWorkplans()
        {
            return UseConverter(c => c.GetWorkplans());
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