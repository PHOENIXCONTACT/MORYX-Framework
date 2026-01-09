// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Identity;
using Moryx.Products.Management.Model;

namespace Moryx.Products.Management;

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
    public IReadOnlyList<Type> RecipeTypes => _recipeInformation.Select(rs => rs.Value.Strategy.TargetType).ToList();

    /// <summary>
    /// Optimized constructor delegate for the different product types
    /// Custom strategies for each product type
    /// </summary>
    public IReadOnlyList<Type> ProductTypes => _typeInformation.Where(ts => ts.Value.Strategy != null)
        .Select(ts => ts.Value.Strategy.TargetType).ToList();

    /// <summary>
    /// Map of product types and their type information
    /// </summary>
    private readonly IDictionary<string, ProductTypeInformation> _typeInformation = new Dictionary<string, ProductTypeInformation>();

    /// <summary>
    /// Constructors for recipes
    /// Strategies for the different recipe types
    /// </summary>
    private readonly IDictionary<string, ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>> _recipeInformation =
        new Dictionary<string, ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>>();

    /// <summary>
    /// Map of custom strategies for each product instance
    /// </summary>
    private readonly IDictionary<string, IProductInstanceStrategy> _instanceStrategies = new Dictionary<string, IProductInstanceStrategy>();

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
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Create type strategies
        var types = ReflectionTool.GetPublicClasses<ProductType>();
        foreach (var type in types)
        {
            _typeInformation.TryAdd(type.FullName, new ProductTypeInformation(type));
        }

        foreach (var config in Config.TypeStrategies)
        {
            var strategy = await StrategyFactory.CreateTypeStrategy(config, cancellationToken);
            if (!_typeInformation.ContainsKey(config.TargetType))
                _typeInformation.Add(config.TargetType, new ProductTypeInformation(strategy.TargetType));
            _typeInformation[config.TargetType].Strategy = strategy;
            _typeInformation[config.TargetType].Constructor = ReflectionTool.ConstructorDelegate<ProductType>(strategy.TargetType);
        }

        // Create instance strategies
        foreach (var config in Config.InstanceStrategies)
        {
            var strategy = await StrategyFactory.CreateInstanceStrategy(config, cancellationToken);
            _instanceStrategies[config.TargetType] = strategy;
        }

        // Create link strategies
        foreach (var config in Config.LinkStrategies)
        {
            if (!_typeInformation.TryGetValue(config.TargetType, out var typeInfo))
                continue;

            var strategy = await StrategyFactory.CreateLinkStrategy(config, cancellationToken);
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
            var strategy = await StrategyFactory.CreateRecipeStrategy(config, cancellationToken);
            if (!_recipeInformation.ContainsKey(config.TargetType))
                _recipeInformation.Add(config.TargetType, new ConstructorStrategyInformation<IProductRecipe, ProductRecipeConfiguration, IProductRecipeStrategy>());

            _recipeInformation[config.TargetType].Strategy = strategy;
            _recipeInformation[config.TargetType].Constructor = ReflectionTool.ConstructorDelegate<IProductRecipe>(strategy.TargetType);
        }
    }

    /// <summary>
    /// Stop the storage
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    #region Recipes

    /// <inheritdoc />
    public async Task<IProductRecipe> LoadRecipeAsync(long recipeId, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var recipeRepo = uow.GetRepository<IProductRecipeRepository>();
        var recipeEntity = recipeRepo.GetByKey(recipeId);
        return recipeEntity != null ? await LoadRecipe(uow, recipeEntity, cancellationToken) : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IProductRecipe>> LoadRecipesAsync(long productId, RecipeClassification classifications, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
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

        var loadRecipeTasks = recipeEntities.Select(async (entity) => await LoadRecipe(uow, entity, cancellationToken)).ToArray();
        var results = await Task.WhenAll(loadRecipeTasks);
        return results;
    }

    /// <summary>
    /// Load recipe object from entity.
    /// </summary>
    private async Task<IProductRecipe> LoadRecipe(IUnitOfWork uow, ProductRecipeEntity recipeEntity, CancellationToken cancellationToken)
    {
        var productRecipe = _recipeInformation[recipeEntity.TypeName].Constructor();

        RecipeStorage.CopyToRecipe(recipeEntity, productRecipe);
        productRecipe.Product = await LoadType(uow, productRecipe.Product.Id, cancellationToken);

        await _recipeInformation[recipeEntity.TypeName].Strategy.LoadRecipeAsync(recipeEntity, productRecipe, cancellationToken);

        return productRecipe;
    }

    /// <inheritdoc/>
    public async Task<long> SaveRecipeAsync(IProductRecipe recipe, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var entity = await SaveRecipe(uow, recipe, cancellationToken);
        uow.SaveChanges();
        recipe.Id = entity.Id;

        return recipe.Id;
    }

    /// <inheritdoc />
    public async Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();

        // Save changes to recipes
        foreach (var recipe in recipes)
        {
            await SaveRecipe(uow, recipe, cancellationToken);
        }

        uow.SaveChanges();
    }

    /// <summary>
    /// Saves <see cref="ProductRecipe"/> to database and return the <see cref="ProductRecipeEntity"/>
    /// </summary>
    private async Task<ProductRecipeEntity> SaveRecipe(IUnitOfWork uow, IProductRecipe recipe, CancellationToken cancellationToken)
    {
        var entity = RecipeStorage.ToRecipeEntity(uow, recipe);

        await _recipeInformation[recipe.RecipeTypeName()].Strategy.SaveRecipeAsync(recipe, entity, cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public Task DeleteRecipeAsync(long recipeId, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();

        // Prepare required repos
        var recipeRepo = uow.GetRepository<IProductRecipeRepository>();

        var deletedRecipe = recipeRepo.GetByKey(recipeId);
        recipeRepo.Remove(deletedRecipe);

        uow.SaveChanges();

        return Task.CompletedTask;
    }

    #endregion

    #region Load product

    public Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var baseSet = uow.GetRepository<IProductTypeRepository>().Linq;
        var productsQuery = query.IncludeDeleted ? baseSet : baseSet.Active();

        // Filter by type
        if (!string.IsNullOrEmpty(query.TypeName))
        {
            if (query.ExcludeDerivedTypes)
                productsQuery = productsQuery.Where(p => p.TypeName == query.TypeName);
            else
            {
                var baseType = _typeInformation[query.TypeName].Type;
                var allTypes = ReflectionTool.GetPublicClasses<ProductType>(type =>
                {
                    // Check if any interface matches
                    if (type.GetInterface(query.TypeName) != null)
                        return true;
                    // Check if type or base type matches
                    if (baseType == null)
                        return false;
                    return baseType.IsAssignableFrom(type);
                }).Select(t => t.FullName);
                productsQuery = productsQuery.Where(p => allTypes.Contains(p.TypeName));
            }

            // Filter by product state
            if (query.RequiredState.HasValue)
            {
                productsQuery = productsQuery.Where(p => ((ProductState)p.CurrentVersion.State).HasFlag(query.RequiredState.Value));
            }

            // Filter by type properties
            if (query.PropertyFilters != null)
            {
                var targetTypeName = query.TypeName.Split(',')[0];
                var typeSearch = _typeInformation[targetTypeName].Strategy;
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
        productsQuery = query.Selector switch
        {
            Selector.Parent => productsQuery.SelectMany(p => p.Parents)
                .Where(p => p.Parent.Deleted == null).Select(link => link.Parent),
            Selector.Parts => productsQuery.SelectMany(p => p.Parts)
                .Where(p => p.Parent.Deleted == null).Select(link => link.Child),
            _ => productsQuery
        };

        // Include current version
        //productsQuery = productsQuery.Include(p => p.CurrentVersion);

        // Execute the query
        var products = productsQuery.OrderBy(p => p.TypeName)
            .ThenBy(p => p.Identifier)
            .ThenBy(p => p.Revision).ToList();

        var results = products.Where(p => _typeInformation.ContainsKey(p.TypeName))
            .Select(p => _typeInformation[p.TypeName].CreateTypeFromEntity(p)).ToArray();

        return Task.FromResult<IReadOnlyList<ProductType>>(results);
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

    public async Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var repo = uow.GetRepository<IProductTypeRepository>();
        var matchingStrategies = _typeInformation.Values.Where(v => v.Strategy != null).Select(p => p.Strategy)
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

        var transformed = entities.Select(entity => Transform(uow, entity, false, cancellationToken, loadedProducts));
        var instances = (await Task.WhenAll(transformed)).OfType<TType>().ToArray();


        // Final check against compiled expression
        var compiledSelector = selector.Compile();
        // Only return matches against compiled expression
        var results = instances.Where(compiledSelector.Invoke).ToArray();
        return results;
    }

    /// <inheritdoc />
    public Task<ProductType> LoadTypeAsync(long id, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        return LoadType(uow, id, cancellationToken);
    }

    private Task<ProductType> LoadType(IUnitOfWork uow, long id, CancellationToken cancellationToken)
    {
        var product = uow.GetRepository<IProductTypeRepository>().GetByKey(id);
        return product != null ? Transform(uow, product, true, cancellationToken) : null;
    }

    /// <inheritdoc />
    public async Task<ProductType> LoadTypeAsync(IIdentity identity, CancellationToken cancellationToken = default)
    {
        if (identity is not ProductIdentity productIdentity)
        {
            throw new NotSupportedException($"Identity of type {identity.GetType()} is not supported. Only {nameof(ProductIdentity)} is supported.");
        }

        using var uow = Factory.Create();
        var productRepo = uow.GetRepository<IProductTypeRepository>();

        var revision = productIdentity.Revision;
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
        return product != null ? await Transform(uow, product, true, cancellationToken) : null;
    }

    private async Task<ProductType> Transform(IUnitOfWork uow, ProductTypeEntity typeEntity, bool full, CancellationToken cancellationToken, IDictionary<long, ProductType> loadedProducts = null)
    {
        // Build cache if this wasn't done before
        loadedProducts ??= new Dictionary<long, ProductType>();

        // Take converted product from dictionary if we already transformed it
        if (loadedProducts.TryGetValue(typeEntity.Id, out var loadedProduct))
            return loadedProduct;

        // Strategy to load product and its parts
        var strategy = _typeInformation[typeEntity.TypeName].Strategy;

        // Load product
        var product = _typeInformation[typeEntity.TypeName].Constructor();
        product.Id = typeEntity.Id;
        product.Name = typeEntity.Name;
        product.State = (ProductState)typeEntity.CurrentVersion.State;
        product.Identity = new ProductIdentity(typeEntity.Identifier, typeEntity.Revision);
        await strategy.LoadTypeAsync(typeEntity.CurrentVersion, product, cancellationToken);

        // Don't load parts and parent for partial view
        if (full)
            await LoadParts(uow, typeEntity, product, loadedProducts, cancellationToken);

        // Assign instance to dictionary of loaded products
        loadedProducts[typeEntity.Id] = product;

        return product;
    }

    /// <summary>
    /// Load all parts of the product
    /// </summary>
    private async Task LoadParts(IUnitOfWork uow, ProductTypeEntity typeEntity, ProductType productType, IDictionary<long, ProductType> loadedProducts,
        CancellationToken cancellationToken)
    {
        // Let's get nasty!
        // Load children
        var type = productType.GetType();
        foreach (var partLink in _typeInformation[productType.ProductTypeName()].PartLinksInformation.Values)
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
                    await part.LoadPartLinkAsync(linkEntity, link, cancellationToken);
                    link.Product = await Transform(uow, linkEntity.Child, true, cancellationToken, loadedProducts);
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
                    await part.LoadPartLinkAsync(linkEntity, link, cancellationToken);
                    link.Product = await Transform(uow, linkEntity.Child, true, cancellationToken, loadedProducts);
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
    public async Task<long> SaveTypeAsync(ProductType modifiedInstance, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var productSaverContext = new ProductPartsSaverContext(uow);
        var entity = await SaveType(productSaverContext, modifiedInstance, cancellationToken);

        uow.SaveChanges();
        modifiedInstance.Id = entity.Id;
        foreach (var item in productSaverContext.PersistentObjectCache)
        {
            item.Key.Id = item.Value.Id;
        }

        return entity.Id;
    }

    private async Task<ProductTypeEntity> SaveType(ProductPartsSaverContext saverContext, ProductType modifiedProductType, CancellationToken cancellationToken)
    {
        var identity = modifiedProductType.Identity;
        if (identity is not ProductIdentity productIdentity)
        {
            throw new NotSupportedException($"Identity of type {identity.GetType()} is not supported. Only {nameof(ProductIdentity)} is supported.");
        }

        var strategy = _typeInformation[modifiedProductType.ProductTypeName()].Strategy
                       ?? throw new InvalidOperationException($"Cannot save product of type {modifiedProductType.GetType().FullName}. No {nameof(IProductTypeStrategy)} is configured for this type in the {nameof(ModuleConfig)}");

        //TODO use uow directly instead of repo if that is possible
        // Get or create entity
        var repo = saverContext.GetRepository<IProductTypeRepository>();
        ProductTypeEntity typeEntity;
        var entities = repo.Linq
            .Where(p => p.Identifier == productIdentity.Identifier && p.Revision == productIdentity.Revision)
            .ToList();
        // If entity does not exist or was deleted, create a new one
        if (entities.All(p => p.Deleted != null))
        {
            typeEntity = repo.Create(productIdentity.Identifier, productIdentity.Revision, modifiedProductType.Name, modifiedProductType.ProductTypeName());
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
        await strategy.SaveTypeAsync(modifiedProductType, typeEntity.CurrentVersion, cancellationToken);
        saverContext.EntityCache.Add(new ProductIdentity(typeEntity.Identifier, typeEntity.Revision), typeEntity);

        // And nasty again!
        var type = modifiedProductType.GetType();

        var linkRepo = saverContext.GetRepository<IPartLinkRepository>();
        foreach (var partLinkInfo in _typeInformation[modifiedProductType.ProductTypeName()].GetAllPartLinks(modifiedProductType))
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
                    await linkStrategy.SavePartLinkAsync(link, linkEntity, cancellationToken);
                    linkEntity.Child = await GetPartEntity(saverContext, link, cancellationToken);
                    saverContext.PersistentObjectCache.Add(link, linkEntity);

                }
                else if (linkEntity != null && link == null) // link was removed
                {
                    await linkStrategy.DeletePartLinkAsync([linkEntity], cancellationToken);
                    linkRepo.Remove(linkEntity);
                }
                else if (linkEntity != null && link != null) // link was modified
                {
                    await linkStrategy.SavePartLinkAsync(link, linkEntity, cancellationToken);
                    linkEntity.Child = await GetPartEntity(saverContext, link, cancellationToken);
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
                await linkStrategy.DeletePartLinkAsync(toDelete, cancellationToken);
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
                    await linkStrategy.SavePartLinkAsync(link, linkEntity, cancellationToken);
                    linkEntity.Child = await GetPartEntity(saverContext, link, cancellationToken);
                    saverContext.PersistentObjectCache.Add(link, linkEntity);
                }
            }
        }

        return typeEntity;
    }

    private async Task<ProductTypeEntity> GetPartEntity(ProductPartsSaverContext saverContext, ProductPartLink link, CancellationToken cancellationToken)
    {
        if (saverContext.EntityCache.ContainsKey((ProductIdentity)link.Product.Identity))
        {
            var part = saverContext.EntityCache[(ProductIdentity)link.Product.Identity];
            saverContext.PersistentObjectCache.Add(link.Product, part);
            return part;
        }

        if (link.Product.Id == 0)
            return await SaveType(saverContext, link.Product, cancellationToken);

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

    public async Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(long[] ids, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        var repo = uow.GetRepository<IProductInstanceRepository>();
        var entities = repo.GetByKeys(ids);

        var results = await TransformInstances(uow, entities, cancellationToken);

        return results;
    }

    public Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(ProductType productType, CancellationToken cancellationToken = default)
    {
        return LoadInstancesAsync<ProductInstance>(pi => pi.Type == productType, cancellationToken);
    }

    public async Task<IReadOnlyList<TInstance>> LoadInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken = default)
    {
        if (IsTypeQuery(selector, out var member, out var value))
        {
            return (await LoadInstancesByType(selector, member, value, cancellationToken)).ToArray();
        }

        var result = await LoadWithStrategy(selector, cancellationToken);
        return result;
    }

    private async Task<IReadOnlyList<TInstance>> LoadInstancesByType<TInstance>(Expression<Func<TInstance, bool>> selector, MemberInfo typeProperty,
        object value, CancellationToken cancellationToken)
    {
        using var uow = Factory.Create();
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

        return (await TransformInstances(uow, entities, cancellationToken)).OfType<TInstance>().ToList();
    }

    private async Task<IReadOnlyList<TInstance>> LoadWithStrategy<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken)
    {
        using var uow = Factory.Create();
        var repo = uow.GetRepository<IProductInstanceRepository>();
        var matchingStrategies = _instanceStrategies.Values
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

        var instances = (await TransformInstances(uow, entities, cancellationToken)).OfType<TInstance>().ToArray();
        // Final check against compiled expression
        var compiledSelector = selector.Compile();
        // Only return matches against compiled expression
        return instances.Where(compiledSelector.Invoke).ToArray();
    }

    /// <summary>
    /// Transform entities to business objects
    /// </summary>
    private async Task<ProductInstance[]> TransformInstances(IUnitOfWork uow, ICollection<ProductInstanceEntity> entities,
        CancellationToken cancellationToken)
    {
        var results = new ProductInstance[entities.Count];

        // Fetch all products we need to load product instances
        var productMap = new Dictionary<long, ProductType>();
        var requiredProducts = entities.Select(e => e.ProductId).Distinct();
        foreach (var productId in requiredProducts)
        {
            productMap[productId] = await LoadType(uow, productId, cancellationToken);
        }

        // Create product instance using the type and fill properties
        var index = 0;
        foreach (var entity in entities)
        {
            var product = productMap[entity.ProductId];
            var instance = product.CreateInstance();

            await TransformInstance(uow, entity, instance, cancellationToken);

            results[index++] = instance;
        }

        return results;
    }

    /// <summary>
    /// Recursive function to transform entities into objects
    /// </summary>
    private async Task TransformInstance(IUnitOfWork uow, ProductInstanceEntity entity, ProductInstance productInstance, CancellationToken cancellationToken)
    {
        productInstance.Id = entity.Id;
        productInstance.State = (ProductInstanceState)entity.State;

        // Transform the instance if it has a dedicated storage
        var productType = productInstance.Type;

        // Check if instances of this type are persisted
        var strategy = _instanceStrategies[productInstance.ProductInstanceTypeName()];
        if (strategy.SkipInstances)
            return;

        // Transform entity to instance
        await strategy.LoadInstanceAsync(entity, productInstance, cancellationToken);

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
            var linkStrategy = _typeInformation[productType.ProductTypeName()].PartLinksInformation[partGroup.Key.Name].Strategy;
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
                    await TransformInstance(uow, partEntity, part, cancellationToken);
                }
            }
            else if (linkStrategy.PartCreation == PartSourceStrategy.FromEntities)
            {
                // Load part using the entity and assign PartLink afterwards
                var partCollection = partEntityGroups[partGroup.Key.Name].ToList();
                var partArticles = await TransformInstances(uow, partCollection, cancellationToken);
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
    public async Task SaveInstancesAsync(ProductInstance[] productInstances, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();

        // Write all to entity objects
        foreach (var instance in productInstances)
            await SaveInstance(uow, instance, cancellationToken);

        // Save transaction
        uow.SaveChanges();
    }

    /// <summary>
    /// Base implementation to save an instance hierarchy.
    /// </summary>
    private async Task<ProductInstanceEntity> SaveInstance(IUnitOfWork uow, ProductInstance productInstance, CancellationToken cancellationToken)
    {
        // Check if this type is persisted
        var strategy = _instanceStrategies[productInstance.ProductInstanceTypeName()];
        if (strategy.SkipInstances)
            return null;

        // Save to entity
        var archived = uow.GetEntity<ProductInstanceEntity>(productInstance);
        archived.State = (int)productInstance.State;
        archived.ProductId = productInstance.Type.Id;
        await strategy.SaveInstanceAsync(productInstance, archived, cancellationToken);

        // Save its parts if they have a dedicated archive
        var partsContainer = ReflectionTool.GetReferences<ProductInstance>(productInstance);
        foreach (var partGroup in partsContainer)
        {
            foreach (var part in partGroup)
            {
                var partEntity = await SaveInstance(uow, part, cancellationToken);
                if (partEntity == null) // Parts are null when they are skipped
                    continue;

                partEntity.Parent = archived;
                partEntity.PartLinkEntityId = part.PartLink.Id;
            }
        }

        return archived;
    }

    public IProductRecipe CreateRecipe(string recipeType)
    {
        if (!_recipeInformation.TryGetValue(recipeType, out var recipeInfo))
            return null;

        if (recipeInfo.Constructor == null)
            return null;

        var result = recipeInfo.Constructor();

        return result;
    }

    public ProductTypeWrapper GetTypeWrapper(string typeName)
    {
        if (!_typeInformation.TryGetValue(typeName, out var typeInfo))
            return null;

        return typeInfo.GetTypeWrapper();
    }

    public Task CheckDatabase(CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
        uow.DbContext.Database.OpenConnection();
        uow.DbContext.Database.CloseConnection();

        return Task.CompletedTask;
    }

    public Task<bool> DeleteTypeAsync(long productId, CancellationToken cancellationToken = default)
    {
        using var uow = Factory.Create();
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
            return Task.FromResult(false);

        // If products would be affected by the removal, we do not remove it
        if (queryResult.parentCount >= 1)
            return Task.FromResult(false);

        // No products affected, so we can remove the product
        productRepo.Remove(queryResult.entity);
        uow.SaveChanges();

        return Task.FromResult(true);
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