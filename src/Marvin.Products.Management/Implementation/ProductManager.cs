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

        private IDictionary<string, Type> _types;

        #endregion

        public ProductManager()
        {
            _types = ReflectionTool.GetPublicClasses<Product>().ToDictionary(t => t.Name, t => t);
        }

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

        public IReadOnlyList<IProduct> GetProducts(ProductQuery query)
        {
            using (var uow = Factory.Create(ContextMode.AllOff))
            {
                var baseSet = uow.GetRepository<IProductEntityRepository>().Linq;
                var productsQuery = query.IncludeDeleted ? baseSet : baseSet.Active();

                // Filter by type
                if (!string.IsNullOrEmpty(query.Type))
                {
                    if (query.ExcludeDerivedTypes)
                        productsQuery = productsQuery.Where(p => p.TypeName == query.Type);
                    else
                    {
                        var queryType = _types[query.Type];
                        var allTypes = ReflectionTool.GetPublicClasses(queryType).Select(t => t.Name);
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
                    productsQuery = productsQuery.Where(p => p.CurrentVersion.Name.ToLower().Contains(query.Name.ToLower()));

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
                // TODO: Move to Storage in AL5 and use TypeWrapper with constructor delegate
                return products.Select(p =>
                {
                    var instance = (Product)Activator.CreateInstance(_types[p.TypeName]);
                    instance.Id = p.Id;
                    instance.Identity = new ProductIdentity(p.Identifier, p.Revision);
                    instance.Name = p.CurrentVersion.Name;
                    return instance;
                }).ToList();
            }
        }

        public IProduct GetProduct(long id)
        {
            return Storage.LoadProduct(id);
        }

        public IProduct GetProduct(ProductIdentity identity)
        {
            return Storage.LoadProduct(identity);
        }

        public long Save(IProduct modifiedInstance)
        {
            var saved = Storage.SaveProduct(modifiedInstance);
            RaiseProductChanged(modifiedInstance);
            return saved;
        }

        public IProduct Create(string type)
        {
            // TODO: Use type wrapper
            var product = (Product)Activator.CreateInstance(_types[type]);
            return product;
        }

        public IProduct Duplicate(long sourceId, ProductIdentity newIdentity)
        {
            // Fetch source object from db
            var duplicate = (Product)Storage.LoadProduct(sourceId);

            // Fetch existing products for identity validation
            var existing = GetProducts(new ProductQuery { Identifier = newIdentity.Identifier });
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

        public IReadOnlyList<IProduct> ImportProducts(string importerName, IImportParameters parameters)
        {
            var importer = _importers.First(i => i.Name == importerName);
            var imported = importer.Import(parameters);
            foreach (var product in imported)
            {
                Save(product);
            }
            return imported;
        }

        public bool DeleteProduct(long productId)
        {
            using (var uow = Factory.Create())
            {
                var productRepo = uow.GetRepository<IProductEntityRepository>();
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

        public Article CreateInstance(IProduct product, bool save)
        {
            var instance = product.CreateInstance();
            if (save)
                SaveArticles(instance);
            return instance;
        }

        public void SaveArticles(params Article[] articles)
        {
            Storage.SaveArticles(articles);
        }

        public Article GetArticle(long id)
        {
            return Storage.LoadArticle(id);
        }

        public IEnumerable<Article> GetArticles(ArticleState state)
        {
            return Storage.LoadArticles(state);
        }

        public IEnumerable<Article> GetArticles(int state)
        {
            return Storage.LoadArticles(state);
        }

        private void RaiseProductChanged(IProduct product)
        {
            // This must never by null
            // ReSharper disable once PossibleNullReferenceException
            ProductChanged(this, product);
        }
        public event EventHandler<IProduct> ProductChanged;
    }
}