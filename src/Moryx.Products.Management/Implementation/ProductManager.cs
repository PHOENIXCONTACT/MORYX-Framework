// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Linq.Expressions;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IProductManager))]
    internal class ProductManager : IProductManager
    {
        #region Dependencies

        public IProductStorage Storage { get; set; }

        public IUnitOfWorkFactory<ProductsContext> Factory { get; set; }

        public IProductImporterFactory ImportFactory { get; set; }

        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields and Properties

        private IList<IProductImporter> _importers;

        public IProductImporter[] Importers => _importers.ToArray();

        private IDictionary<Guid, ImportState> _runningImports = new ConcurrentDictionary<Guid, ImportState>();

        #endregion

        public void Start()
        {
            Storage.CheckDatabase();
            _importers = (from importerConfig in Config.Importers
                          select ImportFactory.Create(importerConfig)).ToList();
        }

        public void Stop()
        {
        }

        public IReadOnlyList<ProductType> LoadTypes(ProductQuery query)
        {
            return Storage.LoadTypes(query);
        }

        public IReadOnlyList<TType> LoadTypes<TType>(Expression<Func<TType, bool>> selector)
        {
            return Storage.LoadTypes(selector);
        }

        public ProductType LoadType(long id)
        {
            return Storage.LoadType(id);
        }

        public ProductType LoadType(IIdentity identity)
        {
            return Storage.LoadType(identity);
        }

        public long SaveType(ProductType modifiedInstance)
        {
            var saved = Storage.SaveType(modifiedInstance);
            //reload the object for correct references
            var loadedType = Storage.LoadType(saved);
            RaiseProductChanged(loadedType);

            return saved;
        }

        public ProductType CreateType(string type)
        {
            var wrapper = Storage.GetTypeWrapper(type);
            if (wrapper == null || wrapper.Constructor == null)
                return (ProductType)TypeTool.CreateInstance<ProductType>(type);
            return wrapper.Constructor();
        }

        public ProductType Duplicate(ProductType template, IIdentity newIdentity)
        {
            if (newIdentity is not ProductIdentity newProductIdentity)
            {
                throw new NotSupportedException($"Identity of type {newIdentity.GetType()} is not supported. Only {nameof(ProductIdentity)} is supported.");
            }

            // Fetch existing products for identity validation
            var existing = LoadTypes(new ProductQuery { Identifier = newIdentity.Identifier });
            // Check if the same revision already exists
            if (existing.Any(e => ((ProductIdentity)e.Identity).Revision == newProductIdentity.Revision))
                throw new IdentityConflictException();
            // If there are any products for this identifier, the source object must be one of them
            if (existing.Any() && template.Identity.Identifier != newIdentity.Identifier)
                throw new IdentityConflictException(true);

            // Reset database id, assign identity and save
            var duplicate = Storage.LoadType(template.Id);
            duplicate.Id = 0;
            duplicate.Identity = newIdentity;
            duplicate.Id = Storage.SaveType(duplicate);

            // Load all recipes and create clones
            foreach (var recipe in Storage.LoadRecipes(template.Id, RecipeClassification.CloneFilter))
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

        public async Task<ProductImportResult> Import(string importerName, object parameters)
        {
            var importer = _importers.First(i => i.Name == importerName);
            var context = new ProductImportContext();
            var result = await importer.ImportAsync(context, parameters);

            HandleResult(result);

            return result;
        }

        internal void HandleResult(ProductImporterResult result)
        {
            if (result.Saved)
                return;

            foreach (var product in result.ImportedTypes)
                SaveType(product);
        }

        public ImportState ImportParallel(string importerName, object parameters)
        {
            var context = new ProductImportContext();
            var session = new ImportState(this) { Session = context.Session };
            _runningImports.Add(context.Session, session);

            var importer = _importers.First(i => i.Name == importerName);
            var task = importer.ImportAsync(context, parameters);
            task.ContinueWith(session.TaskCompleted);

            // Wait for the task unless it is long running
            if (!importer.LongRunning)
                task.Wait(new TimeSpan(0, 0, 0, Config.MaxImporterWait));

            // Return session object, it can be running, completed or faulted in the meantime
            return session;
        }

        public ImportState ImportProgress(Guid session)
        {
            return _runningImports[session];
        }

        public bool DeleteType(long productId)
        {
            using (var uow = Factory.Create())
            {
                var productRepo = uow.GetRepository<IProductTypeRepository>();
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
                uow.SaveChanges();

                return true;
            }
        }

        public ProductInstance CreateInstance(ProductType productType, bool save)
        {
            var instance = productType.CreateInstance();
            if (save)
                SaveInstances(instance);
            return instance;
        }

        public void SaveInstances(params ProductInstance[] productInstances)
        {
            Storage.SaveInstances(productInstances);
        }

        public IReadOnlyList<ProductInstance> GetInstances(long[] ids)
        {
            return Storage.LoadInstances(ids);
        }

        public IReadOnlyList<TInstance> GetInstances<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            return Storage.LoadInstances(selector);
        }

        private void RaiseProductChanged(ProductType productType)
        {
            // This must never by null
            // ReSharper disable once PossibleNullReferenceException
            TypeChanged(this, productType);
        }

        public ProductTypeWrapper GetTypeWrapper(string typeName)
        {
            return Storage.GetTypeWrapper(typeName);
        }

        public event EventHandler<ProductType> TypeChanged;
    }
}
