// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using Moryx.Model;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Model.Repositories;
using Moryx.Tools;
using static Moryx.Products.Management.ProductExpressionHelpers;
using Moryx.Logging;
using Moryx.Products.Management.Implementation.Storage;
using Microsoft.Extensions.Logging;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for product storage. Contains basic functionality to load and save a product.
    /// Also has the possibility to store a version to each save.
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IProductStorage), typeof(IConfiguredTypesProvider))]
    internal class ProductStorage : IProductStorage, IConfiguredTypesProvider
    {
        /// <summary>
        /// Recipe types
        /// </summary>
        public IReadOnlyList<Type> RecipeTypes => RecipeInformation.Select(rs => rs.Value.Strategy.TargetType).ToList();

        /// <summary>
        /// Optimized constructor delegate for the different product types
        /// Custom strategies for each product type
        /// </summary>
        public IReadOnlyList<Type> ProductTypes => TypeInformation.Where(ts => ts.Value.Strategy != null)
            .Select(ts => ts.Value.Strategy.TargetType).ToList();

        protected IDictionary<string, ProductTypeInformation> TypeInformation { get; }
            = new Dictionary<string, ProductTypeInformation>();

        /// <summary>
        /// Constructors for recipes
        /// Strategies for the different recipe types
        /// </summary>
        protected IDictionary<string, ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>> RecipeInformation { get; }
            = new Dictionary<string, ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>>();

        /// <summary>
        /// Map of custom strategies for each product instance
        /// </summary>
        protected IDictionary<string, IProductInstanceStrategy> InstanceStrategies { get; } = new Dictionary<string, IProductInstanceStrategy>();

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
        /// Logger for the product manager module
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Start the storage and load the type strategies
        /// </summary>
        public void Start()
        {
            // Create type strategies
            var types = ReflectionTool.GetPublicClasses<ProductType>();
            foreach (var type in types)
            {
                TypeInformation.TryAdd(type.FullName, new ProductTypeInformation(type));
            }

            foreach (var config in Config.TypeStrategies)
            {
                var strategy = StrategyFactory.CreateTypeStrategy(config);
                if (!TypeInformation.ContainsKey(config.TargetType))
                    TypeInformation.Add(config.TargetType, new ProductTypeInformation(strategy.TargetType));
                TypeInformation[config.TargetType].Strategy = strategy;
                TypeInformation[config.TargetType].Constructor = ReflectionTool.ConstructorDelegate<ProductType>(strategy.TargetType);
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
                if (!TypeInformation.ContainsKey(config.TargetType))
                    continue;
                var typeInfo = TypeInformation[config.TargetType];
                var strategy = StrategyFactory.CreateLinkStrategy(config);
                if (!typeInfo.PartLinksInformation.ContainsKey(config.PartName))
                    typeInfo.PartLinksInformation.Add(config.PartName, new ConstructorStrategyInformation<ProductPartLink, ProductLinkConfiguration, IProductLinkStrategy>());
                var partLinkInfo = typeInfo.PartLinksInformation[config.PartName];
                partLinkInfo.Strategy = strategy;

                var property = strategy.TargetType.GetProperty(config.PartName);
                var linkType = property.PropertyType;
                // Extract element type from collections
                if (typeof(IEnumerable<ProductPartLink>).IsAssignableFrom(linkType))
                {
                    linkType = linkType.GetGenericArguments()[0];
                }
                // Build generic type
                if (linkType.IsInterface && linkType.IsGenericType)
                {
                    var genericElement = linkType.GetGenericArguments()[0];
                    linkType = typeof(ProductPartLink<>).MakeGenericType(genericElement);
                }

                partLinkInfo.Constructor = ReflectionTool.ConstructorDelegate<ProductPartLink>(linkType);
            }

            // Create recipe strategies
            foreach (var config in Config.RecipeStrategies)
            {
                var strategy = StrategyFactory.CreateRecipeStrategy(config);
                if (!RecipeInformation.ContainsKey(config.TargetType))
                    RecipeInformation.Add(config.TargetType, new ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>());
                RecipeInformation[config.TargetType].Strategy = strategy;
                RecipeInformation[config.TargetType].Constructor = ReflectionTool.ConstructorDelegate<IProductRecipe>(strategy.TargetType);
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
                var recipeRepo = uow.GetRepository<IProductRecipeRepository>();
                var recipeEntity = recipeRepo.GetByKey(recipeId);
                return recipeEntity != null ? LoadRecipe(uow, recipeEntity) : null;
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<IProductRecipe> LoadRecipes(long productId, RecipeClassification classifications)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductTypeRepository>();
                var productEntity = repo.GetByKey(productId);
                if (productEntity == null)
                    return null;

                var classificationMask = (int)classifications;
                var recipeEntities = (from recipeEntity in uow.GetRepository<IProductRecipeRepository>().Linq.Active()
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
            var productRecipe = RecipeInformation[recipeEntity.TypeName].Constructor();

            RecipeStorage.CopyToRecipe(recipeEntity, productRecipe);
            productRecipe.Product = LoadType(uow, productRecipe.Product.Id);

            RecipeInformation[recipeEntity.TypeName].Strategy.LoadRecipe(recipeEntity, productRecipe);

            return productRecipe;
        }

        ///
        public long SaveRecipe(IProductRecipe recipe)
        {
            using (var uow = Factory.Create())
            {
                var entity = SaveRecipe(uow, recipe);
                uow.SaveChanges();
                recipe.Id = entity.Id;

                return recipe.Id;
            }
        }

        /// <inheritdoc />
        public void SaveRecipes(IReadOnlyList<IProductRecipe> recipes)
        {
            using var uow = Factory.Create();

            // Save changes to recipes
            foreach (var recipe in recipes)
            {
                SaveRecipe(uow, recipe);
            }

            uow.SaveChanges();
        }

        /// <summary>
        /// Saves <see cref="ProductRecipe"/> to database and return the <see cref="ProductRecipeEntity"/>
        /// </summary>
        protected ProductRecipeEntity SaveRecipe(IUnitOfWork uow, IProductRecipe recipe)
        {
            var entity = RecipeStorage.ToRecipeEntity(uow, recipe);

            RecipeInformation[recipe.GetType().FullName].Strategy.SaveRecipe(recipe, entity);

            return entity;
        }

        /// <inheritdoc />
        public void RemoveRecipe(long recipeId)
        {
            using (var uow = Factory.Create())
            {
                // Prepare required repos
                var recipeRepo = uow.GetRepository<IProductRecipeRepository>();

                var deletedRecipe = recipeRepo.GetByKey(recipeId);
                recipeRepo.Remove(deletedRecipe);

                uow.SaveChanges();
            }
        }

        #endregion

        #region Load product

        public IReadOnlyList<ProductType> LoadTypes(ProductQuery query)
        {
            using var uow = Factory.Create();
            var baseSet = uow.GetRepository<IProductTypeRepository>().Linq;
            var productsQuery = query.IncludeDeleted ? baseSet : baseSet.Active();

            // Filter by type
            if (!string.IsNullOrEmpty(query.Type))
            {
                if (query.ExcludeDerivedTypes)
                    productsQuery = productsQuery.Where(p => p.TypeName == query.Type);
                else
                {
                    var baseType = TypeInformation[query.Type].Type;
                    var allTypes = ReflectionTool.GetPublicClasses<ProductType>(type =>
                    {
                        // Check if any interface matches
                        if (type.GetInterface(query.Type) != null)
                            return true;
                        // Check if type or base type matches
                        if (baseType == null)
                            return false;
                        return baseType.IsAssignableFrom(type);
                    }).Select(t => t.FullName);
                    productsQuery = productsQuery.Where(p => allTypes.Contains(p.TypeName));
                }

                // Filter by type properties properties
                if (query.PropertyFilters != null)
                {
                    var targetTypeName = query.Type.Split(',')[0];
                    var typeSearch = TypeInformation[targetTypeName].Strategy;
                    var targetType = typeSearch.TargetType;
                    // Make generic method for the target type
                    var genericMethod = typeof(IProductTypeStrategy).GetMethod(nameof(IProductTypeStrategy.TransformSelector));
                    var method = genericMethod.MakeGenericMethod(targetType);

                    foreach (var propertyFilter in query.PropertyFilters)
                    {
                        var expression = ConvertPropertyFilter(targetType, propertyFilter);
                        var columnExpression = (Expression<Func<IGenericColumns, bool>>)method.Invoke(typeSearch,
                            [expression]);
                        var versionExpression = AsVersionExpression(columnExpression);
                        productsQuery = productsQuery.Where(versionExpression);
                    }
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
            //productsQuery = productsQuery.Include(p => p.CurrentVersion);

            // Execute the query
            var products = productsQuery.OrderBy(p => p.TypeName)
                .ThenBy(p => p.Identifier)
                .ThenBy(p => p.Revision).ToList();
            return products.Where(p => TypeInformation.ContainsKey(p.TypeName)).Select(p =>
            {
                return TypeInformation[p.TypeName].CreateTypeFromEntity(p);
            }).ToList();
        }

        private static Expression ConvertPropertyFilter(Type targetType, PropertyFilter filter)
        {
            // Product property expression
            var productExpression = Expression.Parameter(targetType);
            var propertyExpresssion = Expression.Property(productExpression, filter.Entry.Identifier);

            var property = targetType.GetProperty(filter.Entry.Identifier);
            var value = Convert.ChangeType(filter.Entry.Value.Current, property.PropertyType);
            var constantExpression = Expression.Constant(value);

            Expression expressionBody;
            switch (filter.Operator)
            {
                case PropertyFilterOperator.Equals:
                    expressionBody = Expression.MakeBinary(ExpressionType.Equal, propertyExpresssion, constantExpression);
                    break;
                case PropertyFilterOperator.GreaterThen:
                    expressionBody = Expression.MakeBinary(ExpressionType.GreaterThan, propertyExpresssion, constantExpression);
                    break;
                case PropertyFilterOperator.LessThen:
                    expressionBody = Expression.MakeBinary(ExpressionType.LessThan, propertyExpresssion, constantExpression);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Expression.Lambda(expressionBody, productExpression);
        }

        public IReadOnlyList<TType> LoadTypes<TType>(Expression<Func<TType, bool>> selector)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductTypeRepository>();
                var matchingStrategies = TypeInformation.Values.Where(v => v.Strategy != null).Select(p => p.Strategy)
                    .Where(i => typeof(TType).IsAssignableFrom(i.TargetType)).ToList();

                IQueryable<ProductTypeEntity> query = null;
                foreach (var typeStrategy in matchingStrategies)
                {
                    var columnExpression = typeStrategy.TransformSelector(selector);
                    var queryFilter = AsVersionExpression(columnExpression);
                    query = query == null
                        ? repo.Linq.Where(queryFilter) // Create query
                        : query.Union(repo.Linq.Where(queryFilter)); // Append query
                }

                // No query or no result => Nothing to do
                List<ProductTypeEntity> entities;
                if (query == null || (entities = query.ToList()).Count == 0)
                    return Array.Empty<TType>();

                var loadedProducts = new Dictionary<long, ProductType>();
                var instances = entities.Select(entity => Transform(uow, entity, false, loadedProducts)).OfType<TType>().ToArray();
                // Final check against compiled expression
                var compiledSelector = selector.Compile();
                // Only return matches against compiled expression
                return instances.Where(compiledSelector.Invoke).ToArray();
            }
        }

        /// <inheritdoc />
        public ProductType LoadType(long id)
        {
            using var uow = Factory.Create();
            return LoadType(uow, id);
        }

        private ProductType LoadType(IUnitOfWork uow, long id)
        {
            var product = uow.GetRepository<IProductTypeRepository>().GetByKey(id);
            return product != null ? Transform(uow, product, true) : null;
        }

        /// <inheritdoc />
        public ProductType LoadType(ProductIdentity identity)
        {
            using var uow = Factory.Create();
            var productRepo = uow.GetRepository<IProductTypeRepository>();

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

            var product = uow.GetRepository<IProductTypeRepository>().Linq.Active()
                .FirstOrDefault(p => p.Identifier == identity.Identifier && p.Revision == revision);
            return product != null ? Transform(uow, product, true) : null;
        }

        private ProductType Transform(IUnitOfWork uow, ProductTypeEntity typeEntity, bool full, IDictionary<long, ProductType> loadedProducts = null, ProductPartLink parentLink = null)
        {
            // Build cache if this wasn't done before
            if (loadedProducts == null)
                loadedProducts = new Dictionary<long, ProductType>();

            // Take converted product from dictionary if we already transformed it
            if (loadedProducts.ContainsKey(typeEntity.Id))
                return loadedProducts[typeEntity.Id];

            // Strategy to load product and its parts
            var strategy = TypeInformation[typeEntity.TypeName].Strategy;

            // Load product
            var product = TypeInformation[typeEntity.TypeName].Constructor();
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
        private void LoadParts(IUnitOfWork uow, ProductTypeEntity typeEntity, ProductType productType, IDictionary<long, ProductType> loadedProducts)
        {
            // Let's get nasty!
            // Load children
            var type = productType.GetType();
            foreach (var partLink in TypeInformation[type.FullName].PartLinksInformation.Values)
            {
                var part = partLink.Strategy;
                object value = null;
                var property = type.GetProperty(part.PropertyName);
                if (typeof(ProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var linkEntity = FindLink(part.PropertyName, typeEntity);
                    if (linkEntity != null)
                    {
                        var link = partLink.Constructor();
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
                        var link = partLink.Constructor();
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
        public long SaveType(ProductType modifiedInstance)
        {
            using (var uow = Factory.Create())
            {
                var productSaverContext = new ProductPartsSaverContext(uow);
                var entity = SaveProduct(productSaverContext, modifiedInstance);

                uow.SaveChanges();
                modifiedInstance.Id = entity.Id;
                foreach (var item in productSaverContext.PersistentObjectCache)
                {
                    item.Key.Id = item.Value.Id;
                }

                return entity.Id;
            }
        }

        private ProductTypeEntity SaveProduct(ProductPartsSaverContext saverContext, ProductType modifiedProductType)
        {
            var strategy = TypeInformation[modifiedProductType.ProductTypeName()].Strategy
                ?? throw new InvalidOperationException($"Cannot save product of type {modifiedProductType.GetType().FullName}. No {nameof(IProductTypeStrategy)} is configured for this type in the {nameof(ModuleConfig)}");

            //TODO use uow directly instead of repo if that is possible
            // Get or create entity
            var repo = saverContext.GetRepository<IProductTypeRepository>();
            var identity = (ProductIdentity)modifiedProductType.Identity;
            ProductTypeEntity typeEntity;
            var entities = repo.Linq
                .Where(p => p.Identifier == identity.Identifier && p.Revision == identity.Revision)
                .ToList();
            // If entity does not exist or was deleted, create a new one
            if (entities.All(p => p.Deleted != null))
            {
                typeEntity = repo.Create(identity.Identifier, identity.Revision, modifiedProductType.Name, modifiedProductType.ProductTypeName());
                saverContext.UnitOfWork.LinkEntityToBusinessObject(modifiedProductType, typeEntity);
            }
            else
            {
                typeEntity = entities.First(p => p.Deleted == null);
                typeEntity.Name = modifiedProductType.Name;
                // Set id in case it was imported under existing material and revision
                modifiedProductType.Id = typeEntity.Id;
            }
            // Check if we need to create a new version
            if (typeEntity.CurrentVersion == null || typeEntity.CurrentVersion.State != (int)modifiedProductType.State || strategy.HasChanged(modifiedProductType, typeEntity.CurrentVersion))
            {
                var version = saverContext.GetRepository<IProductPropertiesRepository>().Create();
                version.State = (int)modifiedProductType.State;
                typeEntity.SetCurrentVersion(version);
            }
            saverContext.PersistentObjectCache.Add(modifiedProductType, typeEntity);
            strategy.SaveType(modifiedProductType, typeEntity.CurrentVersion);
            saverContext.EntityCache.Add(new ProductIdentity(typeEntity.Identifier, typeEntity.Revision), typeEntity);

            // And nasty again!
            var type = modifiedProductType.GetType();

            var linkRepo = saverContext.GetRepository<IPartLinkRepository>();
            foreach (var partLinkInfo in TypeInformation[type.FullName].GetAllPartLinks(modifiedProductType))
            {
                var linkStrategy = partLinkInfo.ProductLinkStrategy;
                if (partLinkInfo.Type == PartLinkType.single)
                {
                    var link = (ProductPartLink)partLinkInfo.Value;
                    var linkEntity = FindLink(linkStrategy.PropertyName, typeEntity);
                    if (linkEntity == null && link != null) // link is new
                    {
                        linkEntity = linkRepo.Create(linkStrategy.PropertyName);
                        saverContext.UnitOfWork.LinkEntityToBusinessObject(link, linkEntity);
                        linkEntity.Parent = typeEntity;
                        linkStrategy.SavePartLink(link, linkEntity);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                        saverContext.PersistentObjectCache.Add(link, linkEntity);

                    }
                    else if (linkEntity != null && link == null) // link was removed
                    {
                        linkStrategy.DeletePartLink([linkEntity]);
                        linkRepo.Remove(linkEntity);
                    }
                    else if (linkEntity != null && link != null) // link was modified
                    {
                        linkStrategy.SavePartLink(link, linkEntity);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                        //                 linkEntity.Id = linkEntity.Child.Id;
                    }
                    // else: link was null and is still null

                }
                else if (partLinkInfo.Type == PartLinkType.list)
                {
                    var links = (IEnumerable<ProductPartLink>)partLinkInfo.Value;
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
                        PartLinkEntity linkEntity;
                        if (link.Id == 0 || (linkEntity = currentEntities.FirstOrDefault(p => p.Id == link.Id)) == null)
                        {
                            linkEntity = linkRepo.Create(linkStrategy.PropertyName);
                            saverContext.UnitOfWork.LinkEntityToBusinessObject(link, linkEntity);
                            linkEntity.Parent = typeEntity;
                        }
                        else
                        {
                            linkEntity = typeEntity.Parts.First(p => p.Id == link.Id);
                        }
                        linkStrategy.SavePartLink(link, linkEntity);
                        linkEntity.Child = GetPartEntity(saverContext, link);
                        saverContext.PersistentObjectCache.Add(link, linkEntity);
                    }
                }
            }

            return typeEntity;
        }

        private ProductTypeEntity GetPartEntity(ProductPartsSaverContext saverContext, ProductPartLink link)
        {
            if (saverContext.EntityCache.ContainsKey((ProductIdentity)link.Product.Identity))
            {
                var part = saverContext.EntityCache[(ProductIdentity)link.Product.Identity];
                saverContext.PersistentObjectCache.Add(link.Product, part);
                return part;
            }

            if (link.Product.Id == 0)
                return SaveProduct(saverContext, link.Product);
            return saverContext.UnitOfWork.GetEntity<ProductTypeEntity>(link.Product);
        }

        /// <summary>
        /// Find the link for this property name
        /// </summary>
        private static PartLinkEntity FindLink(string propertyName, ProductTypeEntity typeEntity)
        {
            return typeEntity.Parts.FirstOrDefault(p => p.PropertyName == propertyName);
        }

        /// <summary>
        /// Find all links for this product name
        /// </summary>
        private static IEnumerable<PartLinkEntity> FindLinks(string propertyName, ProductTypeEntity typeEntity)
        {
            return typeEntity.Parts.Where(p => p.PropertyName == propertyName);
        }

        #endregion

        #region Get instances

        public IReadOnlyList<ProductInstance> LoadInstances(params long[] id)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductInstanceRepository>();
                var entities = repo.GetByKeys(id);
                return TransformInstances(uow, entities);
            }
        }

        public IReadOnlyList<ProductInstance> LoadInstances(ProductType productType)
        {
            return LoadInstances<ProductInstance>(pi => pi.Type == productType);
        }

        public IReadOnlyList<TInstance> LoadInstances<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            if (IsTypeQuery(selector, out var member, out var value))
            {
                return LoadInstancesByType(selector, member, value).ToList();
            }
            else
            {
                return LoadWithStrategy(selector);
            }
        }

        private IReadOnlyList<TInstance> LoadInstancesByType<TInstance>(Expression<Func<TInstance, bool>> selector, MemberInfo typeProperty, object value)
        {
            using (var uow = Factory.Create())
            {
                Expression<Func<ProductInstanceEntity, bool>> instanceSelector;
                // Select by type or type id
                if (typeProperty == null || typeProperty.Name == nameof(ProductType.Id))
                {
                    var productTypeId = (value as ProductType)?.Id ?? (long)value;
                    instanceSelector = i => i.ProductId == productTypeId;
                }
                else if (typeProperty.Name == nameof(ProductType.Name))
                {
                    var productName = (string)value;
                    instanceSelector = i => i.Product.Name == productName;
                }
                else if (typeProperty.Name == nameof(ProductType.Identity))
                {
                    var productIdentity = (ProductIdentity)value;
                    instanceSelector = i => i.Product.Identifier == productIdentity.Identifier && i.Product.Revision == productIdentity.Revision;
                }
                else
                {
                    // TODO: Filter by type specific properties
                    Logger.LogWarning("You tried to load an instance filtering a property ({0}) of the custom type {1}. " +
                        "This is not supported yet and will always return a negative result.", typeProperty.Name, typeProperty.ReflectedType.Name);
                    var productType = typeProperty.ReflectedType;
                    instanceSelector = i => false;
                }

                var repo = uow.GetRepository<IProductInstanceRepository>();
                var entities = repo.Linq.Where(instanceSelector).ToList();

                return TransformInstances(uow, entities).OfType<TInstance>().ToList();
            }
        }

        public IReadOnlyList<TInstance> LoadWithStrategy<TInstance>(Expression<Func<TInstance, bool>> selector)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductInstanceRepository>();
                var matchingStrategies = InstanceStrategies.Values
                   .Where(i => typeof(TInstance).IsAssignableFrom(i.TargetType));

                IQueryable<ProductInstanceEntity> query = null;
                foreach (var instanceStrategy in matchingStrategies)
                {
                    var queryFilter = instanceStrategy.TransformSelector(selector);
                    query = query == null
                        ? repo.Linq.Where(queryFilter).Cast<ProductInstanceEntity>() // Create query
                        : query.Union(repo.Linq.Where(queryFilter).Cast<ProductInstanceEntity>()); // Append query
                }

                // No query or no result => Nothing to do
                List<ProductInstanceEntity> entities;
                if (query == null || (entities = query.ToList()).Count == 0)
                    return Array.Empty<TInstance>();

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
            var productMap = new Dictionary<long, ProductType>();
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
            productInstance.State = (ProductInstanceState)entity.State;

            // Transform the instance if it has a dedicated storage
            var productType = productInstance.Type;

            // Check if instances of this type are persisted
            var strategy = InstanceStrategies[productInstance.GetType().FullName];
            if (strategy.SkipInstances)
                return;

            // Transform entity to instance
            strategy.LoadInstance(entity, productInstance);

            // Group all parts of the instance by the property they belong to
            var partLinks = ReflectionTool.GetReferences<ProductPartLink>(productType)
                .SelectMany(g => g).ToList();
            var partGroups = ReflectionTool.GetReferences<ProductInstance>(productInstance)
                .ToDictionary(p => p.Key, p => p.ToList());
            var partEntityGroups = entity.Parts.GroupBy(p => p.PartLinkEntity.PropertyName)
                .ToDictionary(p => p.Key, p => p.ToList());

            // Load and populate parts
            foreach (var partGroup in partGroups)
            {
                var linkStrategy = TypeInformation[productType.GetType().FullName].PartLinksInformation[partGroup.Key.Name].Strategy;
                if (linkStrategy.PartCreation == PartSourceStrategy.FromPartLink && partEntityGroups.ContainsKey(partGroup.Key.Name))
                {
                    // Update all parts that are also present as entities
                    foreach (var partEntity in partEntityGroups[partGroup.Key.Name])
                    {
                        if (!partGroup.Value.Any())
                        {
                            Logger.LogWarning("No reconstruction of the property {1} possible. You have configured the {0} strategy, but the property was null." +
                                "Please initialize the property in the Initialize method or select the {2} strategy.",
                                nameof(PartSourceStrategy.FromPartLink), partGroup.Key.Name, nameof(PartSourceStrategy.FromEntities));
                            continue;
                        }
                        var part = partGroup.Value.First(p => p.PartLink.Id == partEntity.PartLinkEntityId);
                        TransformInstance(uow, partEntity, part);
                    }
                }
                else if (linkStrategy.PartCreation == PartSourceStrategy.FromEntities)
                {
                    // Load part using the entity and assign PartLink afterwards
                    var partCollection = partEntityGroups[partGroup.Key.Name].ToList();
                    var partArticles = TransformInstances(uow, partCollection);
                    for (var index = 0; index < partArticles.Length; index++)
                    {
                        partArticles[index].PartLink = partLinks.Find(pl => pl?.Id == partCollection[index].PartLinkEntityId.Value);
                    }

                    if (typeof(ProductInstance).IsAssignableFrom(partGroup.Key.PropertyType) && partArticles.Length == 1)
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
            using var uow = Factory.Create();

            // Write all to entity objects
            foreach (var instance in productInstances)
                SaveInstance(uow, instance);

            // Save transaction
            uow.SaveChanges();
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
            var strategy = InstanceStrategies[productInstance.GetType().FullName];
            if (strategy.SkipInstances)
                return null;

            // Save to entity
            var archived = uow.GetEntity<ProductInstanceEntity>(productInstance);
            archived.State = (int)productInstance.State;
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
                    partEntity.PartLinkEntityId = part.PartLink.Id;
                }
            }

            return archived;
        }

        public ProductType CreateTypeInstance(string typeName)
        {
            if (!TypeInformation.ContainsKey(typeName))
                return null;
            var typeInfo = TypeInformation[typeName];
            if (typeInfo.Constructor == null)
                return null;
            return typeInfo.Constructor();
        }

        public IProductRecipe CreateRecipe(string recipeType)
        {
            if (!RecipeInformation.ContainsKey(recipeType))
                return null;
            var recipeInfo = RecipeInformation[recipeType];
            if (recipeInfo.Constructor == null)
                return null;
            return recipeInfo.Constructor();
        }

        public ProductTypeWrapper GetTypeWrapper(string typeName)
        {
            if (!TypeInformation.ContainsKey(typeName))
                return null;
            return TypeInformation[typeName].GetTypeWrapper();
        }

        public void CheckDatabase()
        {
            using (var uow = Factory.Create())
            {
                uow.DbContext.Database.OpenConnection();
                uow.DbContext.Database.CloseConnection();
            }
        }

        #endregion

        private class ProductPartsSaverContext
        {
            public IUnitOfWork UnitOfWork { get; }
            public IDictionary<ProductIdentity, ProductTypeEntity> EntityCache { get; }
            public IDictionary<IPersistentObject, EntityBase> PersistentObjectCache { get; }

            public ProductPartsSaverContext(IUnitOfWork uow)
            {
                UnitOfWork = uow;
                EntityCache = new Dictionary<ProductIdentity, ProductTypeEntity>();
                PersistentObjectCache = new Dictionary<IPersistentObject, EntityBase>();
            }

            public T GetRepository<T>() where T : class, IRepository
            {
                return UnitOfWork.GetRepository<T>();
            }
        }
    }
}
