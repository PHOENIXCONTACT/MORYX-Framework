using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Container;
using Marvin.Model;
using Marvin.Products.Management.Importers;
using Marvin.Products.Management.Modification;
using Marvin.Products.Model;

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

        public void Dispose()
        {
        }

        public IEnumerable<IProduct> GetAll()
        {
            return Storage.LoadProducts();
        }

        public ProductStructureEntry[] ExportTree()
        {
            using (var uow = Factory.Create(ContextMode.AllOff))
            {
                var prodRepo = uow.GetRepository<IProductEntityRepository>();
                var revGroups = from prod in prodRepo.Linq
                                where prod.Deleted == null
                                group prod by prod.MaterialNumber into revisions
                                select revisions;
                var products = (from revGroup in revGroups
                                let highestRev = revGroup.Max(rev => rev.Revision)
                                from product in revGroup
                                let currentVersion = product.CurrentVersion
                                let structureEntry = new
                                {
                                    Latest = product.Revision == highestRev,
                                    Product = new ProductStructureEntry
                                    {
                                        Id = product.Id,
                                        Name = currentVersion.Name,
                                        MaterialNumber = product.MaterialNumber,
                                        Revision = product.Revision,
                                        Type = product.TypeName,
                                        State = (ProductState)currentVersion.State,
                                        BranchType = BranchType.Product
                                    },
                                    Branches = (from part in product.Parts
                                                group part.ChildId by part.PropertyName into partGroups
                                                select partGroups).ToList()

                                }
                                where structureEntry.Latest || product.Parents.Any()
                                group structureEntry by product.TypeName into groups
                                select groups).ToList();

                // Merged buffer
                var allProducts = products.SelectMany(g => g.Select(struc => struc.Product)).ToList();

                // Merge shit together
                var result = new List<ProductStructureEntry>();
                foreach (var productGroup in products)
                {
                    foreach (var productStructure in productGroup)
                    {
                        var index = 0;
                        var product = productStructure.Product;
                        product.Branches = new ProductStructureEntry[productStructure.Branches.Count];
                        foreach (var branchGroup in productStructure.Branches)
                        {
                            // ReSharper disable once AccessToForEachVariableInClosure
                            var parts = allProducts.Where(p => branchGroup.Contains(p.Id)).ToArray();
                            var entry = new ProductStructureEntry
                            {
                                Name = branchGroup.Key,
                                BranchType = BranchType.PartCollector,
                                Branches = parts
                            };
                            product.Branches[index++] = entry;
                        }
                    }
                    result.Add(new ProductStructureEntry
                    {
                        Name = productGroup.Key,
                        BranchType = BranchType.Group,
                        Branches = productGroup.Where(e => e.Latest).Select(e => e.Product).ToArray()
                    });
                }

                return result.ToArray();
            }
        }

        public ProductRevisionEntry[] Revisions(string identifier)
        {
            using (var uow = Factory.Create(ContextMode.AllOff))
            {
                var revisions = (from prod in uow.GetRepository<IProductEntityRepository>().Linq
                                 join revisionHistory in uow.GetRepository<IRevisionHistoryRepository>().Linq
                                     on prod equals revisionHistory.ProductRevision into revisioHistories
                                 from history in revisioHistories.DefaultIfEmpty()
                                 let currentVersion = prod.CurrentVersion
                                 let state = (ProductState)(currentVersion.State)
                                 where prod.Deleted == null && prod.MaterialNumber == identifier
                                 orderby prod.Revision
                                 select new ProductRevisionEntry
                                 {
                                     ProductId = prod.Id,
                                     Revision = prod.Revision,
                                     CreateDate = prod.Created,
                                     State = state,
                                     ReleaseDate = state == ProductState.Released ? (DateTime?)prod.Updated : null,
                                     Comment = history != null ? history.Comment : null
                                 }).ToArray();
                return revisions;
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

        public IProduct Release(long id)
        {
            var release = Storage.LoadProduct(id);
            release.State = ProductState.Released;
            Save(release);
            return release;
        }

        public IProduct CreateRevision(long productId, short revisionNo, string comment)
        {
            // Fetch copy from DB and reset
            var revision = (Product)Storage.LoadProduct(productId);
            revision.Id = 0;

            // Create revision of the product
            using (var uow = Factory.Create())
            {
                // Create new identity
                var currentIdentity = (ProductIdentity)revision.Identity;
                revision.Identity = new ProductIdentity(currentIdentity.Identifier, revisionNo);

                // Reset state
                revision.State = ProductState.Created;

                // Save new revision to db and set id
                revision.Id = Storage.SaveProduct(revision);

                // Create revision history
                var commentEntity = uow.GetRepository<IRevisionHistoryRepository>().Create(comment ?? "No comment!");
                commentEntity.ProductRevisionId = revision.Id;

                uow.Save();
            }

            // Load all recipes and create clones
            // Using int.MaxValue creates a bitmask that excludes ONLY clones
            foreach (var recipe in Storage.LoadRecipes(productId, RecipeClassification.CloneFilter))
            {
                // Clone
                var clone = (IProductRecipe)recipe.Clone();

                // Restore old classification (default, alternative, ...)
                clone.Classification = recipe.Classification & RecipeClassification.CloneFilter;

                // Update product revision
                clone.Product = revision;

                Storage.SaveRecipe(clone);
            }

            RaiseProductChanged(revision);
            return revision;
        }

        public IProduct[] ImportProducts(string importerName, IImportParameters parameters)
        {
            var importer = _importers.First(i => i.Name == importerName);
            var imported = importer.Import(parameters);
            foreach (var product in imported)
            {
                Save(product);
            }
            return imported;
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