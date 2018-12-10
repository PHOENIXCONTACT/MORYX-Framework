using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Serialization;

namespace Marvin.Products.Management.Modification
{
    [Component(LifeCycle.Transient, typeof(IProductConverter))]
    internal class ProductConverter : IProductConverter
    {
        #region Dependency Injection

        public IProductManager ProductManager { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplanManagement WorkplanManagement { get; set; }

        #endregion

        #region Fields and Properties

        // Forward to serialization on injection
        public ICustomization Customization
        {
            get { return ProductSerialization.Customization; }
            set { ProductSerialization.Customization = value; }
        }

        // Null object pattern for identity
        private static readonly ProductIdentity EmptyIdentity = new ProductIdentity(String.Empty, 0);

        private static readonly PartialSerialization<Product> ProductSerialization = new PartialSerialization<Product>();
        private static readonly PartialSerialization<ProductRecipe> RecipeSerialization = new PartialSerialization<ProductRecipe>();

        private readonly List<ProductModel> _productCache = new List<ProductModel>();

        #endregion

        #region To Model

        public ProductModel[] GetRootProducts()
        {
            var products = ProductManager.GetAll();

            return products.Select(p => ConvertProduct(p, true)).ToArray();
        }

        public ProductModel GetProduct(long id)
        {
            var product = ProductManager.GetProduct(id);
            return ConvertProduct(product, false);
        }

        public ProductModel ReleaseProduct(long id)
        {
            var released = ProductManager.Release(id);
            return ConvertProduct(released, false);
        }

        public ProductModel CreateRevision(long id, short revisionNo, string comment)
        {
            var revision = ProductManager.CreateRevision(id, revisionNo, comment);
            return ConvertProduct(revision, false);
        }

        public ProductModel ImportProduct(string importerName, IImportParameters parameters)
        {
            var products = ProductManager.ImportProducts(importerName, parameters);
            return ConvertProduct(products[0], false);
        }

        public ProductModel[] DeleteProduct(long id)
        {
            var product = ProductManager.GetProduct(id);
            var products = ProductManager.DeleteProduct(product);
            return products.Select(p => ConvertProduct(p, true)).ToArray();
        }

        public RecipeModel GetRecipe(long recipeId)
        {
            var recipe = RecipeManagement.Get(recipeId);
            return ConvertRecipe(recipe);
        }

        public RecipeModel CreateRecipe(string recipeType)
        {
            var recipe = Customization.RecipePrototype(recipeType);
            return ConvertRecipe(recipe);
        }

        public RecipeModel GetProductionRecipe(long productId, long workplanId)
        {
            var recipe = RecipeManagement.GetRecipe(productId, workplanId);
            return recipe == null ? null : ConvertRecipe(recipe);
        }

        public RecipeModel CreateProductionRecipe(long productId, long workplanId, string name)
        {
            var recipe = RecipeManagement.Create(productId, workplanId, name);
            return ConvertRecipe(recipe);
        }

        private ProductModel ConvertProduct(IProduct product, bool flat)
        {
            var converted = _productCache.FirstOrDefault(p => p.Id == product.Id);
            if (converted != null)
                return converted;

            // Base object
            var identity = (ProductIdentity)product.Identity ?? EmptyIdentity;
            converted = new ProductModel
            {
                Id = product.Id,
                Type = product.Type,
                Name = product.Name,
                State = product.State,
                Identifier = identity.Identifier,
                Revision = identity.Revision
            };

            if (flat)
                return converted;

            // Properties
            var properties = product.GetType().GetProperties();
            converted.Properties = EntryConvert.EncodeObject(product, ProductSerialization);

            // Files
            converted.Files = ConvertFiles(product, properties);

            // Recipes
            var recipes = RecipeManagement.GetAllByProduct(product);
            converted.Recipes = recipes.Select(ConvertRecipe).ToArray();

            // Parts
            ConvertParts(product, properties, converted);

            _productCache.Add(converted);
            return converted;
        }

        private static ProductFile[] ConvertFiles(IProduct product, IEnumerable<PropertyInfo> properties)
        {
            var files = (from property in properties
                         where property.PropertyType == typeof(ProductFile)
                         select (ProductFile)property.GetValue(product)).ToArray();
            return files;
        }

        private void ConvertParts(IProduct product, IEnumerable<PropertyInfo> properties, ProductModel converted)
        {
            var connectors = new List<PartConnector>();
            foreach (var property in properties)
            {
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType) && property.Name != nameof(Product.ParentLink))
                {
                    var link = (IProductPartLink)property.GetValue(product);
                    var partModel = ConvertPart(link);
                    var connector = new PartConnector
                    {
                        Name = property.Name,
                        Type = FetchProductType(property.PropertyType),
                        Parts = partModel != null ? new []{partModel} : new PartModel[0],
                        PropertyTemplates = EntryConvert.EncodeClass(property.PropertyType, ProductSerialization)
                    };
                    connectors.Add(connector);
                }
                else if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(property.PropertyType))
                {
                    var links = (IEnumerable<IProductPartLink>)property.GetValue(product);
                    var linkType = property.PropertyType.GetGenericArguments()[0];
                    var connector = new PartConnector
                    {
                        IsCollection = true,
                        Name = property.Name,
                        Type = FetchProductType(linkType),
                        Parts = links.Select(ConvertPart).ToArray(),
                        PropertyTemplates = EntryConvert.EncodeClass(linkType, ProductSerialization)
                    };
                    connectors.Add(connector);
                }
            }
            converted.Parts = connectors.ToArray();
        }

        private PartModel ConvertPart(IProductPartLink link)
        {
            // No link, no DTO!
            if (link == null)
                return null;

            var part = new PartModel
            {
                Product = ConvertProduct(link.Product, false),
                Properties = EntryConvert.EncodeObject(link, ProductSerialization)
            };
            return part;
        }

        private static string FetchProductType(Type linkType)
        {
            var partLinkInterface = linkType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IProductPartLink<>));
            var prodType = partLinkInterface.GetGenericArguments()[0];
            return prodType.Name;
        }

        private static RecipeModel ConvertRecipe(IRecipe recipe)
        {
            // Transform to DTO and transmit
            var converted = new RecipeModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Type = recipe.Type,
                State = recipe.State,
                Revision = recipe.Revision,
                Ingredients = EntryConvert.EncodeObject(recipe, RecipeSerialization),
                IsDefault = recipe.Classification == RecipeClassification.Default
            };

            //TODO: provide known types for recipe dto
            var wpRecipe = recipe as IWorkplanRecipe;
            if (wpRecipe != null)
            {
                converted.WorkplanId = wpRecipe.Workplan.Id;
            }

            return converted;
        }

        #endregion

        #region Convert back

        public ProductModel Save(ProductModel productModel)
        {
            var product = ConvertProductBack(productModel);
            productModel.Id = ProductManager.Save(product);
            return productModel;
        }

        public IProductRecipe ConvertRecipeBack(RecipeModel recipe)
        {
            var productRecipe = recipe.Id == 0 ? Customization.RecipePrototype(recipe.Type) : RecipeManagement.Get(recipe.Id);
            productRecipe.Name = recipe.Name;
            productRecipe.Revision = recipe.Revision;
            productRecipe.State = recipe.State;
            EntryConvert.UpdateInstance(productRecipe, recipe.Ingredients, RecipeSerialization);
            return productRecipe;
        }

        private IProduct ConvertProductBack(ProductModel product)
        {
            // Fetch instance and copy base values
            var converted = (Product)ProductManager.GetProduct(product.Id);
            converted.Identity = new ProductIdentity(product.Identifier, product.Revision);
            converted.Name = product.Name;
            converted.State = product.State;

            // Copy extended properties
            var properties = converted.GetType().GetProperties();
            EntryConvert.UpdateInstance(converted, product.Properties, ProductSerialization);

            ConvertFilesBack(converted, product, properties);

            // Save recipes
            var recipes = product.Recipes.Select(ConvertRecipeBack).ToList();
            RecipeManagement.Save(product.Id, recipes);

            // Convert parts
            foreach (var partConnector in product.Parts)
            {
                var prop = properties.First(p => p.Name == partConnector.Name);
                var value = prop.GetValue(converted);
                if (partConnector.IsCollection)
                    UpdateCollection((IList)value, partConnector.Parts);
                else if (partConnector.Parts.Length == 1)
                    UpdateReference((IProductPartLink)value, partConnector.Parts[0]);
            }

            return converted;
        }

        private void UpdateCollection(IList value, IEnumerable<PartModel> parts)
        {
            // Clear old values
            value.Clear();

            var elemType = value.GetType().GetGenericArguments()[0];
            foreach (var part in parts)
            {
                var link = (IProductPartLink)Activator.CreateInstance(elemType);
                EntryConvert.UpdateInstance(link, part.Properties);
                link.Product = ConvertProductBack(part.Product);
                value.Add(link);
            }
        }

        private void UpdateReference(IProductPartLink value, PartModel part)
        {
            EntryConvert.UpdateInstance(value, part.Properties);
            value.Product = ConvertProductBack(part.Product);
        }

        private static void ConvertFilesBack(object converted, ProductModel product, PropertyInfo[] properties)
        {
            foreach (var fileModel in product.Files)
            {
                var prop = properties.First(p => p.Name == fileModel.Name);
                prop.SetValue(converted, fileModel.File);
            }
        }
        #endregion
    }
}