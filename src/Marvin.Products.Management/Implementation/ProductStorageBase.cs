using System;
using System.Collections;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;
using System.Collections.Generic;
using System.Linq;
using Marvin.Tools;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Base class for product storage. Contains basic functionality to load and save a product. 
    /// Also has the possibility to store a version to each save.
    /// </summary>
    public abstract class ProductStorageBase : IProductStorage
    {
        /// <summary>
        /// Override with your merge factory
        /// </summary>
        public abstract IUnitOfWorkFactory Factory { get; set; }

        #region Type mapping

        /// <summary>
        /// Build map of product types to delegates
        /// </summary>
        protected abstract IProductTypeStrategy[] BuildMap();

        private IDictionary<string, IProductTypeStrategy> _delegateMap;
        /// <summary>
        /// Map of custom delegates for each product type
        /// </summary>
        protected IDictionary<string, IProductTypeStrategy> TypeStrategies
        {
            get { return _delegateMap ?? (_delegateMap = BuildMap().ToDictionary(ts => ts.TargetType, ts => ts)); }
        }

        #endregion

        #region Recipes

        /// 
        public IProductRecipe LoadRecipe(long recipeId)
        {
            using (var uow = Factory.Create())
            {
                var recipeRepo = uow.GetRepository<IProductRecipeEntityRepository>();
                var recipeEntity = recipeRepo.GetByKey(recipeId);

                return LoadRecipe(uow, recipeEntity);
            }
        }

        /// 
        public IReadOnlyList<IProductRecipe> LoadRecipes(long productId, RecipeClassification classifications)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IProductEntityRepository>();
                var productEntity = repo.GetByKey(productId);
                if (productEntity == null)
                    return null;

                var classificationMask = (int)classifications;
                var recipeEntities = (from recipeEntity in uow.GetRepository<IProductRecipeEntityRepository>().Linq
                                      let classificationValue = recipeEntity.Classification
                                      where recipeEntity.ProductId == productId && (classificationValue & classificationMask) == classificationValue
                                      select recipeEntity).ToArray();

                return recipeEntities.Select(entity => LoadRecipe(uow, entity)).ToArray();
            }
        }

        /// <summary>
        /// Load recipe object from entity. 
        /// </summary>
        private IProductRecipe LoadRecipe(IUnitOfWork uow, ProductRecipeEntity recipeEntity)
        {
            var productRecipe = LoadCustomRecipe(uow, recipeEntity);

            RecipeStorage.CopyToRecipe(recipeEntity, productRecipe);
            productRecipe.Product = LoadProduct(uow, productRecipe.Product.Id);

            return productRecipe;
        }

        /// <summary>
        /// Loads the custom recipe from the given database entity
        /// </summary>
        protected virtual IProductRecipe LoadCustomRecipe(IUnitOfWork uow, ProductRecipeEntity recipeEntity)
        {
            return new ProductRecipe();
        }

        /// 
        public long SaveRecipe(IProductRecipe recipe)
        {
            using (var uow = Factory.Create())
            {
                SaveRecipe(uow, recipe);
                uow.Save();
                return recipe.Id;
            }
        }

        /// <summary>
        /// Saves <see cref="ProductRecipe"/> to database and return the <see cref="ProductRecipeEntity"/>
        /// </summary>
        protected virtual ProductRecipeEntity SaveRecipe(IUnitOfWork uow, IProductRecipe recipe)
        {
            return RecipeStorage.SaveRecipe(uow, recipe);
        }

        /// 
        public void SaveRecipes(long productId, ICollection<IProductRecipe> recipes)
        {
            using (var uow = Factory.Create())
            {
                // Prepare required repos
                var prodRepo = uow.GetRepository<IProductEntityRepository>();
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

                uow.Save();
            }
        }

        #endregion

        #region Load product

        /// 
        public IProduct LoadProduct(long id)
        {
            using (var uow = Factory.Create())
            {
                return LoadProduct(uow, id);
            }
        }

        private IProduct LoadProduct(IUnitOfWork uow, long id)
        {
            var product = uow.GetRepository<IProductEntityRepository>().GetByKey(id);
            if (product == null)
                throw new ProductNotFoundException(id);
            return Transform(uow, product, true);
        }

        /// 
        public IProduct LoadProduct(ProductIdentity identity)
        {
            using (var uow = Factory.Create())
            {
                var productRepo = uow.GetRepository<IProductEntityRepository>();

                var revision = identity.Revision;
                // If the latest revision was requested, replace it with the highest current revision
                if (revision == ProductIdentity.LatestRevision)
                    revision = productRepo.Linq.Where(p => p.MaterialNumber == identity.Identifier).Max(p => p.Revision);

                var product = uow.GetRepository<IProductEntityRepository>().GetByIdentity(identity.Identifier, revision);
                return product != null ? Transform(uow, product, true) : null;
            }
        }

        /// 
        public IReadOnlyList<IProduct> LoadProducts()
        {
            using (var uow = Factory.Create())
            {
                var products = ProductList(uow);
                return products.Select(p => Transform(uow, p, false)).ToList();
            }
        }

        /// <summary>
        /// Filter the products shown in product lists
        /// </summary>
        protected virtual ICollection<ProductEntity> ProductList(IUnitOfWork uow)
        {
            var repo = uow.GetRepository<IProductEntityRepository>();

            // All products that are producible
            var products = (from product in repo.Linq
                            where product.Deleted == null
                            where product.Recipes.Any() | !product.Parents.Any()
                            orderby product.MaterialNumber, product.Revision ascending
                            select product
                        ).ToList();

            return products;
        }


        /// <inheritdoc />
        public IProduct TransformProduct(IUnitOfWork context, ProductEntity entity, bool full)
        {
            return Transform(context, entity, full);
        }

        private IProduct Transform(IUnitOfWork uow, ProductEntity entity, bool full, IDictionary<long, IProduct> loadedProducts = null, IProductPartLink parentLink = null)
        {
            // Build cache if this wasn't done before
            if (loadedProducts == null)
                loadedProducts = new Dictionary<long, IProduct>();

            // Take converted product from dictionary if we already transformed it
            if (loadedProducts.ContainsKey(entity.Id))
                return loadedProducts[entity.Id];

            // Strategy to load product and its parts
            var strategy = TypeStrategies[entity.TypeName];

            // To correctly restore the parent relation and build a valid object tree
            // we recursively move up the tree and later extract our reference using the
            // part name.
            PartLink[] parentRelations;
            if (full && parentLink == null && strategy.ParentLoading > ParentLoadBehaviour.Ignore
                && (parentRelations = entity.Parents.Where(p => p.Parent.Deleted == null).ToArray()).Length == 1)
            {
                parentLink = LoadParentLink(uow, entity, strategy, parentRelations[0], loadedProducts);
                if (strategy.ParentLoading >= ParentLoadBehaviour.Full) // For mode full our tree was loaded during parent-load
                    return parentLink.Product;
            }

            // Load product
            var product = strategy.LoadProduct(uow, entity);

            // Don't load parts and parent for partial view
            if (full)
            {
                ((Product)product).ParentLink = parentLink;
                if (parentLink != null && parentLink.Product == null)
                    parentLink.Product = product; // If the parent link was created and not passed we must set the reference for consitency between both modes
                LoadParts(uow, entity, strategy, product, loadedProducts);
            }

            // Assign instance to dictionary of loaded products
            loadedProducts[entity.Id] = product;

            return product;
        }

        private IProductPartLink LoadParentLink(IUnitOfWork uow, ProductEntity entity, IProductTypeStrategy strategy, PartLink parentRelation, IDictionary<long, IProduct> loadedProducts)
        {
            // Load parent
            var parent = Transform(uow, parentRelation.Parent, strategy.ParentLoading >= ParentLoadBehaviour.Full, loadedProducts);
            // Extract ourself from it
            var selfReference = parent.GetType().GetProperty(parentRelation.PropertyName);

            // For full loading we can simply extract the back reference from the parent
            if (strategy.ParentLoading == ParentLoadBehaviour.Full)
            {
                var value = selfReference.GetValue(parent);
                var valueCollection = value as IEnumerable<IProductPartLink>;
                return valueCollection == null ? (IProductPartLink)value : valueCollection.First(link => link.Product.Id == entity.Id);
            }

            // Partial or flat loading is a little trickier
            var parentLinkStrategy = TypeStrategies[parent.Type].Parts.First(p => p.Name == parentRelation.PropertyName);
            var parentLink = parentLinkStrategy.Load(uow, parentRelation);
            parentLink.Parent = parent;

            // For the flat strategy we only set this one link on the parent
            if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(selfReference.PropertyType))
            {
                var linkCollection = (IList)Activator.CreateInstance(selfReference.PropertyType);
                linkCollection.Add(parentLink);
                selfReference.SetValue(parent, linkCollection);
            }
            else
                selfReference.SetValue(parent, parentLink);

            return parentLink;
        }

        /// <summary>
        /// Load all parts of the product
        /// </summary>
        private void LoadParts(IUnitOfWork uow, ProductEntity entity, IProductTypeStrategy strategy, IProduct product, IDictionary<long, IProduct> loadedProducts)
        {
            // Let's get nasty!
            // Load children
            var type = product.GetType();
            foreach (var part in strategy.Parts)
            {
                object value = null;
                var property = type.GetProperty(part.Name);
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var linkEntity = FindLink(part.Name, entity);
                    if (linkEntity != null)
                    {
                        value = LoadPart(uow, product, part, linkEntity, loadedProducts);
                    }
                }
                else if (typeof(IList).IsAssignableFrom(property.PropertyType))
                {
                    var linkEntities = FindLinks(part.Name, entity);
                    var links = (IList)Activator.CreateInstance(property.PropertyType);
                    foreach (var linkEntity in linkEntities)
                    {
                        var link = LoadPart(uow, product, part, linkEntity, loadedProducts);
                        links.Add(link);
                    }
                    value = links;
                }
                property.SetValue(product, value);
            }
        }

        private IProductPartLink LoadPart(IUnitOfWork uow, IProduct parent, ILinkStrategy linkStrategy, PartLink linkEntity, IDictionary<long, IProduct> loadedProducts)
        {
            var link = linkStrategy.Load(uow, linkEntity);
            link.Parent = parent;
            link.Product = (Product)Transform(uow, linkEntity.Child, true, loadedProducts, link);
            return link;
        }

        #endregion

        #region Save product

        /// <summary>
        /// Save a product to the database
        /// </summary>
        public long SaveProduct(IProduct modifiedInstance)
        {
            using (var uow = Factory.Create())
            {
                var entity = SaveProduct(uow, modifiedInstance);

                uow.Save();

                return entity.Id;
            }
        }

        private ProductEntity SaveProduct(IUnitOfWork uow, IProduct modifiedInstance)
        {
            var strategy = TypeStrategies[modifiedInstance.Type];

            // Load product
            var entity = strategy.SaveProduct(uow, modifiedInstance);

            // And nasty again!
            var type = modifiedInstance.GetType();
            foreach (var linkStrategy in strategy.Parts)
            {
                var property = type.GetProperty(linkStrategy.Name);
                var value = property.GetValue(modifiedInstance);
                if (typeof(IProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var link = (IProductPartLink)value;
                    var linkEntity = FindLink(linkStrategy.Name, entity);
                    if (linkEntity == null && link != null) // link is new
                    {
                        linkEntity = linkStrategy.Create(uow, link);
                        linkEntity.Parent = entity;
                        EntityIdListener.Listen(linkEntity, link);
                        linkEntity.Child = GetPartEntity(uow, linkStrategy, link);
                    }
                    else if (linkEntity != null && link == null) // link was removed
                    {
                        linkStrategy.Delete(uow, new[] { linkEntity });
                    }
                    else if (linkEntity != null && link != null) // link was modified
                    {
                        linkStrategy.Update(uow, link, linkEntity);
                        linkEntity.Child = GetPartEntity(uow, linkStrategy, link);
                    }
                    // else: link was null and is still null

                }
                else if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(property.PropertyType))
                {
                    var links = (IEnumerable<IProductPartLink>)value;
                    // Delete the removed ones
                    var toDelete = (from link in entity.Parts
                                    where link.PropertyName == linkStrategy.Name
                                    where links.All(l => l.Id != link.Id)
                                    select link).ToArray();
                    linkStrategy.Delete(uow, toDelete);
                    // Save those curently active
                    foreach (var link in links)
                    {
                        PartLink linkEntity;
                        if (link.Id == 0)
                        {
                            linkEntity = linkStrategy.Create(uow, link);
                            linkEntity.Parent = entity;
                            EntityIdListener.Listen(linkEntity, link);
                        }
                        else
                        {
                            linkEntity = entity.Parts.First(p => p.Id == link.Id);
                            linkStrategy.Update(uow, link, linkEntity);
                        }
                        linkEntity.Child = GetPartEntity(uow, linkStrategy, link);
                    }
                }
            }

            return entity;
        }

        private ProductEntity GetPartEntity(IUnitOfWork uow, ILinkStrategy strategy, IProductPartLink link)
        {
            return strategy.RecursivePartSaving || link.Product.Id == 0 ? SaveProduct(uow, link.Product) : uow.GetEntity<ProductEntity>(link.Product);
        }

        /// <summary>
        /// Find the link for this property name
        /// </summary>
        private static PartLink FindLink(string propertyName, ProductEntity product)
        {
            return product.Parts.FirstOrDefault(p => p.PropertyName == propertyName);
        }

        /// <summary>
        /// Find all links for this product name
        /// </summary>
        private static IEnumerable<PartLink> FindLinks(string propertyName, ProductEntity product)
        {
            return product.Parts.Where(p => p.PropertyName == propertyName);
        }

        #endregion

        #region Get articles

        /// <summary>
        /// Get an article with the given id.
        /// </summary>
        /// <param name="id">The id for the article which should be searched for.</param>
        /// <returns>The article with the id when it exists.</returns>
        public Article LoadArticle(long id)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IArticleEntityRepository>();
                var entity = repo.GetByKey(id);
                return TransformArticles(uow, new[] { entity })[0];
            }
        }

        /// <summary>
        /// Gets a list of articles by a given state
        /// </summary>
        public IEnumerable<Article> LoadArticles(ArticleState state)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IArticleEntityRepository>();
                var entities = repo.Linq.Where(a => (a.State & (int)state) > 0).ToList();
                return TransformArticles(uow, entities);
            }
        }

        /// <summary>
        /// Gets a list of articles by a given state
        /// </summary>
        public IEnumerable<Article> LoadArticles(int combinedState)
        {
            using (var uow = Factory.Create())
            {
                var repo = uow.GetRepository<IArticleEntityRepository>();
                var entities = repo.GetAllByState(combinedState);
                return TransformArticles(uow, entities);
            }
        }

        /// <summary>
        /// Transform entities to business objects
        /// </summary>
        private Article[] TransformArticles(IUnitOfWork uow, ICollection<ArticleEntity> entities)
        {
            var results = new Article[entities.Count];

            // Fetch all products we need to load articles
            var productMap = new Dictionary<long, IProduct>();
            var requiredProducts = entities.Select(e => e.ProductId).Distinct();
            foreach (var productId in requiredProducts)
            {
                productMap[productId] = LoadProduct(uow, productId);
            }

            // Create article instance using the type and fill properties
            var index = 0;
            foreach (var entity in entities)
            {
                var product = productMap[entity.ProductId];
                var article = product.CreateInstance();

                TransformArticle(uow, entity, article);

                results[index++] = article;
            }

            return results;
        }

        /// <summary>
        /// Recursive function to transform entities into objects
        /// </summary>
        private void TransformArticle(IUnitOfWork uow, ArticleEntity entity, Article article)
        {
            // Transform the article if it has a dedicated storage
            var product = article.Product;

            // Check if instances of this type are persisted
            var strategy = TypeStrategies[product.Type];
            if (strategy.SkipArticles)
                return;

            // Transfrom entity to article
            strategy.LoadArticle(uow, entity, article);

            // Group all parts of the article by the property they belong to
            var parts = ((IArticleParts)article).Parts;
            var partGroups = entity.Parts.GroupBy(p => p.PartLink.PropertyName);

            // Load and populate parts
            foreach (var partGroup in partGroups)
            {
                var linkStrategy = strategy.Parts.First(p => p.Name == partGroup.Key);
                if (linkStrategy.PartCreation == PartSourceStrategy.FromPartlink)
                {
                    // Load prepared part from the parts list
                    foreach (var partEntity in partGroup)
                    {
                        var part = parts.First(p => p.PartLinkId == partEntity.PartLinkId);
                        TransformArticle(uow, partEntity, part.Article);
                    }
                }
                else
                {
                    // Load part using only the entity
                    var partCollection = partGroup.ToList();
                    var partArticles = TransformArticles(uow, partCollection);
                    for (var index = 0; index < partArticles.Length; index++)
                    {
                        var partArticle = partArticles[index];
                        var partEntity = partCollection[index];

                        ((IArticleParts)partArticle).PartLinkId = partEntity.PartLinkId.Value;
                        var partWrapper = new ArticlePart(partGroup.Key, partArticle);
                        parts.Add(partWrapper);
                    }
                }
            }
        }

        #endregion

        #region Save Article

        /// <summary>
        /// Updates the database from the article instance
        /// </summary>
        public void SaveArticles(Article[] articles)
        {
            using (var uow = Factory.Create())
            {
                // Write all to entity objects
                foreach (var article in articles)
                {
                    SaveArticle(uow, article);
                }

                // Save transaction
                uow.Save();
            }
        }

        /// <summary>
        /// Base implementation to save an article hierarchy.
        /// </summary>
        /// <param name="uow">An open unit of work</param>
        /// <param name="article">The article to save</param>
        /// <returns>The article entity.</returns>
        private ArticleEntity SaveArticle(IUnitOfWork uow, Article article)
        {
            // Check if this type is persisted
            var strategy = TypeStrategies[article.Product.Type];
            if (strategy.SkipArticles)
                return null;

            // Save to entity
            var archived = strategy.SaveArticle(uow, article);

            // Save its parts if the have a dedicated archive
            var partsContainer = (IArticleParts)article;
            foreach (var namedPart in partsContainer.Parts)
            {
                var part = SaveArticle(uow, namedPart.Article);
                if (part == null) // Parts are null when they are skipped
                    continue;

                part.Parent = archived;
                part.PartLinkId = namedPart.PartLinkId;
            }

            return archived;
        }

        #endregion
    }
}