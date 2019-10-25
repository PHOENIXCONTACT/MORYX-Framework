using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Management.Importers;
using Marvin.Products.Model;
using Marvin.Tools;

namespace Marvin.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IProductManager))]
    internal class ProductManager : IProductManager
    {
        #region Dependencies

        public IProductStorage Storage { get; set; }

        public IUnitOfWorkFactory Factory { get; set; }

        public IProductImporterFactory ImportFactory { get; set; }

        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields and Properties

        private IList<IProductImporter> _importers;

        public IProductImporter[] Importers => _importers.ToArray();

        #endregion
        
        public void Start()
        {
            _importers = (from importerConfig in Config.Importers
                          select ImportFactory.Create(importerConfig)).ToList();
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }

        public IReadOnlyList<IProductType> GetTypes(ProductQuery query)
        {
            return Storage.GetProductTypes(query);
        }

        public IProductType GetType(long id)
        {
            return Storage.LoadProductType(id);
        }

        public IProductType GetType(ProductIdentity identity)
        {
            return Storage.LoadProductType(identity);
        }

        public long SaveType(IProductType modifiedInstance)
        {
            var saved = Storage.SaveProduct(modifiedInstance);
            RaiseProductChanged(modifiedInstance);
            return saved;
        }

        public IProductType CreateType(string type)
        {
            // TODO: Use type wrapper
            var productType = ReflectionTool.GetPublicClasses<ProductType>(t => t.Name == type).First();
            var product = (ProductType)Activator.CreateInstance(productType);
            return product;
        }

        public IProductType Duplicate(long sourceId, ProductIdentity newIdentity)
        {
            // Fetch source object from db
            var duplicate = (ProductType)Storage.LoadProductType(sourceId);

            // Fetch existing products for identity validation
            var existing = GetTypes(new ProductQuery { Identifier = newIdentity.Identifier });
            // Check if the same revision already exists
            if (existing.Any(e => ((ProductIdentity)e.Identity).Revision == newIdentity.Revision))
                throw new IdentityConflictException();
            // If there are any products for this identifier, the source object must be one of them
            if (existing.Any() && duplicate.Identity.Identifier != newIdentity.Identifier)
                throw new IdentityConflictException(true);

            // Reset database id, assign identity and save
            duplicate.Id = 0;
            duplicate.Identity = newIdentity;
            duplicate.Id = Storage.SaveProduct(duplicate);

            // Load all recipes and create clones
            // Using int.MaxValue creates a bitmask that excludes ONLY clones
            foreach (var recipe in Storage.LoadRecipes(sourceId, RecipeClassification.CloneFilter))
            {
                // Clone
                var clone = (IProductRecipe)recipe.Clone();

                // Restore old classification (default, alternative, ...)
                clone.Classification = recipe.Classification & RecipeClassification.CloneFilter;

                // Update product revision
                clone.Product = duplicate;

                Storage.SaveRecipe(clone);
            }

            RaiseProductChanged(duplicate);
            return duplicate;
        }

        public IReadOnlyList<IProductType> ImportTypes(string importerName, IImportParameters parameters)
        {
            var importer = _importers.First(i => i.Name == importerName);
            var imported = importer.Import(parameters);
            foreach (var product in imported)
            {
                SaveType(product);
            }
            return imported;
        }

        public bool DeleteType(long productId)
        {
            using (var uow = Factory.Create())
            {
                var productRepo = uow.GetRepository<IProductTypeEntityRepository>();
                var queryResult = (from entity in productRepo.Linq
                                   where entity.Id == productId
                                   select new
                                   {
                                       entity,
                                       parentCount = entity.Parents.Count
                                   }).FirstOrDefault();
                // No match, nothing removed!
                if (queryResult == null)
                    return false;

                // If products would be affected by the removal, we do not remove it
                if (queryResult.parentCount >= 1)
                    return false;

                // No products affected, so we can remove the product
                productRepo.Remove(queryResult.entity);
                uow.Save();

                return true;
            }
        }

        public ProductInstance CreateInstance(IProductType productType, bool save)
        {
            var instance = productType.CreateInstance();
            if (save)
                SaveInstances(instance);
            return instance;
        }

        public void SaveInstances(params ProductInstance[] productInstances)
        {
            Storage.SaveArticles(productInstances);
        }

        public ProductInstance GetInstance(long id)
        {
            return Storage.LoadArticle(id);
        }

        public IEnumerable<ProductInstance> GetInstances(ProductInstanceState state)
        {
            return Storage.LoadArticles((int)state);
        }

        public IEnumerable<ProductInstance> GetInstances(int state)
        {
            return Storage.LoadArticles(state);
        }

        private void RaiseProductChanged(IProductType productType)
        {
            // This must never by null
            // ReSharper disable once PossibleNullReferenceException
            TypeChanged(this, productType);
        }
        public event EventHandler<IProductType> TypeChanged;
    }
}