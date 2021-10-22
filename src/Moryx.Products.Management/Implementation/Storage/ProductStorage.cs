// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using Moryx.Model;
using Moryx.Products.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for product storage. Contains basic functionality to load and save a product.
    /// Also has the possibility to store a version to each save.
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IProductStorage))]
    internal class ProductStorage : IProductStorage
    {
        /// <summary>
        /// Optimized constructor delegate for the different product types
        /// </summary>
        protected IDictionary<string, Func<ProductType>> TypeConstructors { get; } = new Dictionary<string, Func<ProductType>>();

        /// <summary>
        /// Map of custom strategies for each product type
        /// </summary>
        protected IDictionary<string, IProductTypeStrategy> TypeStrategies { get; } = new Dictionary<string, IProductTypeStrategy>();

        /// <summary>
        /// Map of custom strategies for each product instance
        /// </summary>
        protected IDictionary<string, IProductInstanceStrategy> InstanceStrategies { get; } = new Dictionary<string, IProductInstanceStrategy>();

        /// <summary>
        /// Constructors for partlink objects
        /// </summary>
        protected IDictionary<string, Func<IProductPartLink>> LinkConstructors { get; } = new Dictionary<string, Func<IProductPartLink>>();

        /// <summary>
        /// Map of custom strategies for each product part link
        /// </summary>
        protected IDictionary<string, IDictionary<string, IProductLinkStrategy>> LinkStrategies = new LinkStrategyCache();

        /// <summary>
        /// Constructors for recipes
        /// </summary>
        protected IDictionary<string, Func<IProductRecipe>> RecipeConstructors { get; } = new Dictionary<string, Func<IProductRecipe>>();

        /// <summary>
        /// Strategies for the different recipe types
        /// </summary>
        protected IDictionary<string, IProductRecipeStrategy> RecipeStrategies { get; } = new Dictionary<string, IProductRecipeStrategy>();

        /// <summary>
        /// Override with your merge factory
        /// </summary>
        public IUnitOfWorkFactory<ProductsContext> Factory { get; set; }

        /// <summary>
        /// Factory to create <see cref="IProductTypeStrategy"/>
        /// </summary>
        public IStorageStrategyFactory StrategyFactory { get; set; }

        /// <summary>
        /// Configuration of the product manager module
        /// Necessary to access the type mappings
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <summary>
        /// Start the storage and load the type strategies
        /// </summary>
        public void Start()
        {
            // Create type strategies
            foreach (var config in Config.TypeStrategies)
            {
                var strategy = StrategyFactory.CreateTypeStrategy(config);
                TypeStrategies[config.TargetType] = strategy;
                TypeConstructors[config.TargetType] = ReflectionTool.ConstructorDelegate<ProductType>(strategy.TargetType);
            }
            // Create instance strategies
            foreach (var config in Config.InstanceStrategies)
            {
                var strategy = StrategyFactory.CreateInstanceStrategy(config);
                InstanceStrategies[config.TargetType] = strategy;
            }
            // Create link strategies
            foreach (var config in Config.LinkStrategies)
            {
                var strategy = StrategyFactory.CreateLinkStrategy(config);
                LinkStrategies[config.TargetType][config.PartName] = strategy;

                var property = strategy.TargetType.GetProperty(config.PartName);
                var linkType = property.PropertyType;
                // Extract element type from collections
                if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(linkType))
                {
                    linkType = linkType.GetGenericArguments()[0];
                }
                // Build generic type
                if (linkType.IsInterface && linkType.IsGenericType)
                {
                    var genericElement = linkType.GetGenericArguments()[0];
                    linkType = typeof(ProductPartLink<>).MakeGenericType(genericElement);
                }

                LinkConstructors[$"{config.TargetType}.{config.PartName}"] = ReflectionTool.ConstructorDelegate<IProductPartLink>(linkType);
            }
            // Create recipe strategies
            foreach (var config in Config.RecipeStrategies)
            {
                var strategy = StrategyFactory.CreateRecipeStrategy(config);
                RecipeStrategies[config.TargetType] = strategy;

                RecipeConstructors[config.TargetType] = ReflectionTool.ConstructorDelegate<IProductRecipe>(strategy.TargetType);
            }
        }

        /// <summary>
        /// Stop the storage
        /// </summary>
        public void Stop()
        {
        }

        #region Recipes

        /// <inheritdoc />
        public IProductRecipe LoadRecipe(long recipeId)
        {
            using (var uow = Factory.Create())
            {
                var recipeRepo = uow.GetRepository<IProductRecipeEntityRepository>();
                var recipeEntity = recipeRepo.GetByKey(recipeId);
                return recipeEntity != null ? LoadRecipe(uow, recipeEntity) : null;
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<IProductRecipe> LoadRecipes(long productId, RecipeClassification classifications)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductTypeEntityRepository>();
                var productEntity = repo.GetByKey(productId);
                if (productEntity == null)
                    return null;

                var classificationMask = (int)classifications;
                var recipeEntities = (from recipeEntity in uow.GetRepository<IProductRecipeEntityRepository>().Linq.Active()
                                      let classificationValue = recipeEntity.Classification
                                      where recipeEntity.ProductId == productId
                                      where classificationValue >= 0 // We never return clones in this query
                                      where (classificationValue & classificationMask) == classificationValue

                                      select recipeEntity).ToArray();

                return recipeEntities.Select(entity => LoadRecipe(uow, entity)).ToArray();
            }
        }

        /// <summary>
        /// Load recipe object from entity.
        /// </summary>
        private IProductRecipe LoadRecipe(IUnitOfWork uow, ProductRecipeEntity recipeEntity)
        {
            var productRecipe = RecipeConstructors[recipeEntity.Type]();

            RecipeStorage.CopyToRecipe(recipeEntity, productRecipe);
            productRecipe.Product = LoadType(uow, productRecipe.Product.Id);

            RecipeStrategies[recipeEntity.Type].LoadRecipe(recipeEntity, productRecipe);

            return productRecipe;
        }

        ///
        public long SaveRecipe(IProductRecipe recipe)
        {
            using (var uow = Factory.Create())
            {
                SaveRecipe(uow, recipe);
                uow.SaveChanges();
                return recipe.Id;
            }
        }

        /// <summary>
        /// Saves <see cref="ProductRecipe"/> to database and return the <see cref="ProductRecipeEntity"/>
        /// </summary>
        protected ProductRecipeEntity SaveRecipe(IUnitOfWork uow, IProductRecipe recipe)
        {
            var entity = RecipeStorage.SaveRecipe(uow, recipe);

            RecipeStrategies[recipe.GetType().Name].SaveRecipe(recipe, entity);

            return entity;
        }

        /// <inheritdoc />
        public void SaveRecipes(long productId, ICollection<IProductRecipe> recipes)
        {
            using (var uow = Factory.Create())
            {
                // Prepare required repos
                var prodRepo = uow.GetRepository<IProductTypeEntityRepository>();
                var recipeRepo = uow.GetRepository<IProductRecipeEntityRepository>();

                // Save changes to recipes
                foreach (var recipe in recipes)
                {
                    SaveRecipe(uow, recipe);
                }

                var prod = prodRepo.GetByKey(productId);

                // Find deleted ones and remove from db
                var deleted = from dbRecipe in prod.Recipes
                              where recipes.All(r => r.Id != dbRecipe.Id)
                              select dbRecipe;
                recipeRepo.RemoveRange(deleted);

                uow.SaveChanges();
            }
        }

        #endregion

        #region Load product

        public IReadOnlyList<IProductType> LoadTypes(ProductQuery query)
        {
            using (var uow = Factory.Create(ContextMode.AllOff))
            {
                var baseSet = uow.GetRepository<IProductTypeEntityRepository>().Linq;
                var productsQuery = query.IncludeDeleted ? baseSet : baseSet.Active();

                // Filter by type
                if (!string.IsNullOrEmpty(query.Type))
                {
                    if (query.ExcludeDerivedTypes)
                        productsQuery = productsQuery.Where(p => p.TypeName == query.Type);
                    else
                    {
                        var allTypes = ReflectionTool.GetPublicClasses<ProductType>(pt =>
                        {
                            // TODO: Clean this up with full name and proper type compatibility
                            var type = pt;
                            // Check if any interface matches
                            if (type.GetInterfaces().Any(inter => inter.Name == query.Type))
                                return true;
                            // Check if type or base type matches
                            while (type != null)
                            {
                                if (type.Name == query.Type)
                                    return true;
                                type = type.BaseType;
                            }
                            return false;
                        }).Select(t => t.Name);
                        productsQuery = productsQuery.Where(p => allTypes.Contains(p.TypeName));
                    }
                }

                // Filter by identifier
                if (!string.IsNullOrEmpty(query.Identifier))
                {
                    var identifierMatches = Regex.Match(query.Identifier, "(?<startCard>\\*)?(?<filter>\\w*)(?<endCard>\\*)?");
                    var identifier = identifierMatches.Groups["filter"].Value.ToLower();
                    if (identifierMatches.Groups["startCard"].Success && identifierMatches.Groups["endCard"].Success)
                        productsQuery = productsQuery.Where(p => p.Identifier.ToLower().Contains(identifier));
                    else if (identifierMatches.Groups["startCard"].Success)
                        productsQuery = productsQuery.Where(p => p.Identifier.ToLower().EndsWith(identifier));
                    else if (identifierMatches.Groups["endCard"].Success)
                        productsQuery = productsQuery.Where(p => p.Identifier.ToLower().StartsWith(identifier));
                    else
                        productsQuery = productsQuery.Where(p => p.Identifier.ToLower() == identifier);
                }

                // Filter by revision
                if (query.RevisionFilter == RevisionFilter.Latest)
                {
                    var compareSet = baseSet.Active();
                    productsQuery = productsQuery.Where(p => p.Revision == compareSet.Where(compare => compare.Identifier == p.Identifier).Max(compare => compare.Revision));
                }
                else if (query.RevisionFilter == RevisionFilter.Specific)
                {
                    productsQuery = productsQuery.Where(p => p.Revision == query.Revision);
                }

                // Filter by name
                if (!string.IsNullOrEmpty(query.Name))
                    productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower()));

                // Filter by recipe
                if (query.RecipeFilter == RecipeFilter.WithRecipe)
                    productsQuery = productsQuery.Where(p => p.Recipes.Any());
                else if (query.RecipeFilter == RecipeFilter.WithoutRecipes)
                    productsQuery = productsQuery.Where(p => p.Recipes.Count == 0);

                // Apply selector
                switch (query.Selector)
                {
                    case Selector.Parent:
                        productsQuery = productsQuery.SelectMany(p => p.Parents).Where(p => p.Parent.Deleted == null)
                            .Select(link => link.Parent);
                        break;
                    case Selector.Parts:
                        productsQuery = productsQuery.SelectMany(p => p.Parts).Where(p => p.Parent.Deleted == null)
                            .Select(link => link.Child);
                        break;
                }

                // Include current version
                productsQuery = productsQuery.Include(p => p.CurrentVersion);

                // Execute the query
                var products = productsQuery.OrderBy(p => p.TypeName)
                    .ThenBy(p => p.Identifier)
                    .ThenBy(p => p.Revision).ToList();
                // TODO: Use TypeWrapper with constructor delegate and isolate basic property conversion
                return products.Where(p => TypeConstructors.ContainsKey(p.TypeName)).Select(p =>
                {
                    var instance = TypeConstructors[p.TypeName]();
                    instance.Id = p.Id;
                    instance.Identity = new ProductIdentity(p.Identifier, p.Revision);
                    instance.Name = p.Name;
                    return instance;
                }).ToList();
            }
        }

        /// <inheritdoc />
        public IProductType LoadType(long id)
        {
            using (var uow = Factory.Create())
            {
                return LoadType(uow, id);
            }
        }

        private IProductType LoadType(IUnitOfWork uow, long id)
        {
            var product = uow.GetRepository<IProductTypeEntityRepository>().GetByKey(id);
            return product != null ? Transform(uow, product, true) : null;
        }

        /// <inheritdoc />
        public IProductType LoadType(ProductIdentity identity)
        {
            using (var uow = Factory.Create())
            {
                var productRepo = uow.GetRepository<IProductTypeEntityRepository>();

                var revision = identity.Revision;
                // If the latest revision was requested, replace it with the highest current revision
                if (revision == ProductIdentity.LatestRevision)
                {
                    // Get all revisions of this product
                    var revisions = productRepo.Linq.Active()
                        .Where(p => p.Identifier == identity.Identifier)
                        .Select(p => p.Revision).ToList();
                    if (revisions.Any())
                        revision = revisions.Max();
                    else
                        return null;
                }

                var product = uow.GetRepository<IProductTypeEntityRepository>().Linq.Active()
                    .FirstOrDefault(p => p.Identifier == identity.Identifier && p.Revision == revision);
                return product != null ? Transform(uow, product, true) : null;
            }
        }

        /// <inheritdoc />
        public IProductType TransformType(IUnitOfWork context, ProductTypeEntity typeEntity, bool full)
        {
            return Transform(context, typeEntity, full);
        }

        private IProductType Transform(IUnitOfWork uow, ProductTypeEntity typeEntity, bool full, IDictionary<long, IProductType> loadedProducts = null, IProductPartLink parentLink = null)
        {
            // Build cache if this wasn't done before
            if (loadedProducts == null)
                loadedProducts = new Dictionary<long, IProductType>();

            // Take converted product from dictionary if we already transformed it
            if (loadedProducts.ContainsKey(typeEntity.Id))
                return loadedProducts[typeEntity.Id];

            // Strategy to load product and its parts
            var strategy = TypeStrategies[typeEntity.TypeName];

            // Load product
            var product = TypeConstructors[typeEntity.TypeName]();
            product.Id = typeEntity.Id;
            product.Name = typeEntity.Name;
            product.State = (ProductState)typeEntity.CurrentVersion.State;
            product.Identity = new ProductIdentity(typeEntity.Identifier, typeEntity.Revision);
            strategy.LoadType(typeEntity.CurrentVersion, product);

            // Don't load parts and parent for partial view
            if (full)
                LoadParts(uow, typeEntity, product, loadedProducts);

            // Assign instance to dictionary of loaded products
            loadedProducts[typeEntity.Id] = product;

            return product;
        }

        /// <summary>
        /// Load all parts of the product
        /// </summary>
        private void LoadParts(IUnitOfWork uow, ProductTypeEntity typeEntity, IProductType productType, IDictionary<long, IProductType> loadedProducts)
        {
            // Let's get nasty!
            // Load children
            var type = productType.GetType();
            foreach (var part in LinkStrategies[type.Name].Values)
            {
                object value = null;
                var property = type.GetProperty(part.PropertyName);
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var linkEntity = FindLink(part.PropertyName, typeEntity);
                    if (linkEntity != null)
                    {
                        var link = LinkConstructors[$"{type.Name}.{property.Name}"]();
                        link.Id = linkEntity.Id;
                        part.LoadPartLink(linkEntity, link);
                        link.Product = (ProductType)Transform(uow, linkEntity.Child, true, loadedProducts, link);
                        value = link;
                    }
                }
                else if (typeof(IList).IsAssignableFrom(property.PropertyType))
                {
                    var linkEntities = FindLinks(part.PropertyName, typeEntity);
                    var links = (IList)Activator.CreateInstance(property.PropertyType);
                    foreach (var linkEntity in linkEntities)
                    {
                        var link = LinkConstructors[$"{type.Name}.{property.Name}"]();
                        link.Id = linkEntity.Id;
                        part.LoadPartLink(linkEntity, link);
                        link.Product = (ProductType)Transform(uow, linkEntity.Child, true, loadedProducts, link);
                        links.Add(link);
                    }
                    value = links;
                }
                property.SetValue(productType, value);
            }
        }

        #endregion

        #region Save product

        /// <summary>
        /// Save a product to the database
        /// </summary>
        public long SaveType(IProductType modifiedInstance)
        {
            using (var uow = Factory.Create())
            {
                var productSaverContext = new ProductPartsSaverContext(uow);
                var entity = SaveProduct(productSaverContext, modifiedInstance);

                uow.SaveChanges();

                return entity.Id;
            }
        }

        private ProductTypeEntity SaveProduct(ProductPartsSaverContext saverContext, IProductType modifiedInstance)
        {
            var strategy = TypeStrategies[modifiedInstance.GetType().Name];

            // Get or create entity
            var repo = saverContext.GetRepository<IProductTypeEntityRepository>();
            var identity = (ProductIdentity)modifiedInstance.Identity;
            ProductTypeEntity typeEntity;
            var entities = repo.Linq
                .Where(p => p.Identifier == identity.Identifier && p.Revision == identity.Revision)
                .ToList();
            // If entity does not exist or was deleted, create a new one
            if (entities.All(p => p.Deleted != null))
            {
                typeEntity = repo.Create(identity.Identifier, identity.Revision, modifiedInstance.Name, modifiedInstance.GetType().Name);
                EntityIdListener.Listen(typeEntity, modifiedInstance);
            }
            else
            {
                typeEntity = entities.First(p => p.Deleted == null);
                typeEntity.Name = modifiedInstance.Name;
                // Set id in case it was imported under existing material and revision
                modifiedInstance.Id = typeEntity.Id;
            }
            // Check if we need to create a new version
            if (typeEntity.CurrentVersion == null || typeEntity.CurrentVersion.State != (int)modifiedInstance.State || strategy.HasChanged(modifiedInstance, typeEntity.CurrentVersion))
            {
                var version = saverContext.GetRepository<IProductPropertiesRepository>().Create();
                version.State = (int)modifiedInstance.State;
                typeEntity.SetCurrentVersion(version);
            }

            strategy.SaveType(modifiedInstance, typeEntity.CurrentVersion);
            saverContext.EntityCache.Add(new ProductIdentity(typeEntity.Identifier,typeEntity.Revision),typeEntity);

            // And nasty again!
            var type = modifiedInstance.GetType();
            var linkRepo = saverContext.GetRepository<IPartLinkRepository>();
            foreach (var linkStrategy in LinkStrategies[strategy.TargetType.Name].Values)
            {
                var property = type.GetProperty(linkStrategy.PropertyName);
                var value = property.GetValue(modifiedInstance);
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var link = (IProductPartLink)value;
                    var linkEntity = FindLink(linkStrategy.PropertyName, typeEntity);
                    if (linkEntity == null && link != null) // link is new
                    {
                        linkEntity = linkRepo.Create(linkStrategy.PropertyName);
                        linkEntity.Parent = typeEntity;
                        linkStrategy.SavePartLink(link, linkEntity);
                        EntityIdListener.Listen(linkEntity, link);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                    }
                    else if (linkEntity != null && link == null) // link was removed
                    {
                        linkStrategy.DeletePartLink(new IGenericColumns[] { linkEntity });
                        linkRepo.Remove(linkEntity);
                    }
                    else if (linkEntity != null && link != null) // link was modified
                    {
                        linkStrategy.SavePartLink(link, linkEntity);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                    }
                    // else: link was null and is still null

                }
                else if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(property.PropertyType))
                {
                    var links = (IEnumerable<IProductPartLink>)value;
                    // Delete the removed ones
                    var toDelete = (from link in typeEntity.Parts
                                    where link.PropertyName == linkStrategy.PropertyName
                                    where links.All(l => l.Id != link.Id)
                                    select link).ToArray();
                    linkStrategy.DeletePartLink(toDelete);
                    linkRepo.RemoveRange(toDelete);

                    // Save those currently active
                    var currentEntities = FindLinks(linkStrategy.PropertyName, typeEntity).ToArray();
                    foreach (var link in links)
                    {
                        PartLink linkEntity;
                        if (link.Id == 0 || (linkEntity = currentEntities.FirstOrDefault(p => p.Id == link.Id)) == null)
                        {
                            linkEntity = linkRepo.Create(linkStrategy.PropertyName);
                            linkEntity.Parent = typeEntity;
                            EntityIdListener.Listen(linkEntity, link);
                        }
                        else
                        {
                            linkEntity = typeEntity.Parts.First(p => p.Id == link.Id);
                        }
                        linkStrategy.SavePartLink(link, linkEntity);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                    }
                }
            }

            return typeEntity;
        }

        private ProductTypeEntity GetPartEntity(ProductPartsSaverContext saverContext, IProductPartLink link)
        {
            if (saverContext.EntityCache.ContainsKey((ProductIdentity) link.Product.Identity))
            {
                var part = saverContext.EntityCache[(ProductIdentity) link.Product.Identity];
                EntityIdListener.Listen(part,link.Product);
                return part;
            }

            if (link.Product.Id == 0)
                return SaveProduct(saverContext, link.Product);
            return saverContext.UnitOfWork.GetEntity<ProductTypeEntity>(link.Product);
        }

        /// <summary>
        /// Find the link for this property name
        /// </summary>
        private static PartLink FindLink(string propertyName, ProductTypeEntity typeEntity)
        {
            return typeEntity.Parts.FirstOrDefault(p => p.PropertyName == propertyName);
        }

        /// <summary>
        /// Find all links for this product name
        /// </summary>
        private static IEnumerable<PartLink> FindLinks(string propertyName, ProductTypeEntity typeEntity)
        {
            return typeEntity.Parts.Where(p => p.PropertyName == propertyName);
        }

        #endregion

        #region Get instances

        public IReadOnlyList<ProductInstance> LoadInstances(params long[] id)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductInstanceEntityRepository>();
                var entities = repo.GetByKeys(id);
                return TransformInstances(uow, entities);
            }
        }

        public IReadOnlyList<TInstance> LoadInstances<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductInstanceEntityRepository>();

                var entities = new List<ProductInstanceEntity>();
                var matchingStrategies = InstanceStrategies.Values
                    .Where(i => typeof(TInstance).IsAssignableFrom(i.TargetType));
                foreach (var instanceStrategy in matchingStrategies)
                {
                    var queryFilter = instanceStrategy.TransformSelector(selector);
                    entities.AddRange(repo.Linq.Where(queryFilter).Cast<ProductInstanceEntity>());
                }

                var instances = TransformInstances(uow, entities).OfType<TInstance>().ToArray();
                // Final check against compiled expression
                var compiledSelector = selector.Compile();
                // Only return matches against compiled expression
                return instances.Where(compiledSelector.Invoke).ToArray();
            }
        }

        /// <summary>
        /// Transform entities to business objects
        /// </summary>
        private ProductInstance[] TransformInstances(IUnitOfWork uow, ICollection<ProductInstanceEntity> entities)
        {
            var results = new ProductInstance[entities.Count];

            // Fetch all products we need to load product instances
            var productMap = new Dictionary<long, IProductType>();
            var requiredProducts = entities.Select(e => e.ProductId).Distinct();
            foreach (var productId in requiredProducts)
            {
                productMap[productId] = LoadType(uow, productId);
            }

            // Create product instance using the type and fill properties
            var index = 0;
            foreach (var entity in entities)
            {
                var product = productMap[entity.ProductId];
                var instance = product.CreateInstance();

                TransformInstance(uow, entity, instance);

                results[index++] = instance;
            }

            return results;
        }

        /// <summary>
        /// Recursive function to transform entities into objects
        /// </summary>
        private void TransformInstance(IUnitOfWork uow, ProductInstanceEntity entity, ProductInstance productInstance)
        {
            productInstance.Id = entity.Id;

            // Transform the instance if it has a dedicated storage
            var productType = productInstance.Type;

            // Check if instances of this type are persisted
            var strategy = InstanceStrategies[productInstance.GetType().Name];
            if (strategy.SkipInstances)
                return;

            // Transform entity to instance
            strategy.LoadInstance(entity, productInstance);

            // Group all parts of the instance by the property they belong to
            var partLinks = ReflectionTool.GetReferences<IProductPartLink>(productType)
                .SelectMany(g => g).ToList();
            var partGroups = ReflectionTool.GetReferences<ProductInstance>(productInstance)
                .ToDictionary(p => p.Key, p => p.ToList());
            var partEntityGroups = entity.Parts.GroupBy(p => p.PartLink.PropertyName)
                .ToDictionary(p => p.Key, p => p.ToList());

            // Load and populate parts
            foreach (var partGroup in partGroups)
            {
                var linkStrategy = LinkStrategies[productType.GetType().Name][partGroup.Key.Name];
                if (linkStrategy.PartCreation == PartSourceStrategy.FromPartlink && partEntityGroups.ContainsKey(partGroup.Key.Name))
                {
                    // Update all parts that are also present as entities
                    foreach (var partEntity in partEntityGroups[partGroup.Key.Name])
                    {
                        var part = partGroup.Value.First(p => p.PartLink.Id == partEntity.PartLinkId);
                        TransformInstance(uow, partEntity, part);
                    }
                }
                else if(linkStrategy.PartCreation == PartSourceStrategy.FromEntities)
                {
                    // Load part using the entity and assign PartLink afterwards
                    var partCollection = partEntityGroups[partGroup.Key.Name].ToList();
                    var partArticles = TransformInstances(uow, partCollection);
                    for (var index = 0; index < partArticles.Length; index++)
                    {
                        partArticles[index].PartLink = partLinks.Find(pl => pl?.Id == partCollection[index].PartLinkId.Value);
                    }

                    if (typeof(ProductInstance).IsAssignableFrom(partGroup.Key.PropertyType) && partArticles.Length == 0)
                    {
                        partGroup.Key.SetValue(productInstance, partArticles[0]);
                    }
                    else
                    {
                        var elementType = partGroup.Key.PropertyType.GetGenericArguments()[0];
                        var listType = typeof(List<>).MakeGenericType(elementType);
                        var list = (IList)Activator.CreateInstance(listType);
                        foreach (var partArticle in partArticles)
                        {
                            list.Add(partArticle);
                        }
                    }
                }
            }
        }

        #endregion

        #region Save instances

        /// <summary>
        /// Updates the database from the product instance
        /// </summary>
        public void SaveInstances(ProductInstance[] productInstances)
        {
            using (var uow = Factory.Create())
            {
                // Write all to entity objects
                foreach (var instance in productInstances)
                {
                    SaveInstance(uow, instance);
                }

                // Save transaction
                uow.SaveChanges();
            }
        }

        /// <summary>
        /// Base implementation to save an instance hierarchy.
        /// </summary>
        /// <param name="uow">An open unit of work</param>
        /// <param name="productInstance">The instance to save</param>
        /// <returns>The instance entity.</returns>
        private ProductInstanceEntity SaveInstance(IUnitOfWork uow, ProductInstance productInstance)
        {
            // Check if this type is persisted
            var strategy = InstanceStrategies[productInstance.GetType().Name];
            if (strategy.SkipInstances)
                return null;

            // Save to entity
            var archived = uow.GetEntity<ProductInstanceEntity>(productInstance);
            archived.ProductId = productInstance.Type.Id;
            strategy.SaveInstance(productInstance, archived);

            // Save its parts if they have a dedicated archive
            var partsContainer = ReflectionTool.GetReferences<ProductInstance>(productInstance);
            foreach (var partGroup in partsContainer)
            {
                foreach (var part in partGroup)
                {
                    var partEntity = SaveInstance(uow, part);
                    if (partEntity == null) // Parts are null when they are skipped
                        continue;

                    partEntity.Parent = archived;
                    partEntity.PartLinkId = part.PartLink.Id;
                }
            }

            return archived;
        }

        #endregion

        /// <summary>
        /// Minimal helper for the double indexer used for the link strategy cache
        /// </summary>
        private class LinkStrategyCache : Dictionary<string, IDictionary<string, IProductLinkStrategy>>, IDictionary<string, IDictionary<string, IProductLinkStrategy>>
        {
            IDictionary<string, IProductLinkStrategy> IDictionary<string, IDictionary<string, IProductLinkStrategy>>.this[string key]
            {
                get { return ContainsKey(key) ? this[key] : (this[key] = new Dictionary<string, IProductLinkStrategy>()); }
                set { /*You can not set the internal cache*/ }
            }
        }

        private class ProductPartsSaverContext
        {
            public IUnitOfWork UnitOfWork { get; }
            public IDictionary<ProductIdentity, ProductTypeEntity> EntityCache { get; }

            public ProductPartsSaverContext(IUnitOfWork uow)
            {
                UnitOfWork = uow;
                EntityCache = new Dictionary<ProductIdentity, ProductTypeEntity>();
            }

            public T GetRepository<T>() where T : class, IRepository
            {
                return UnitOfWork.GetRepository<T>();
            }
        }
    }
}
