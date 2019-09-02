using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Products.Management;
using Marvin.Products.Model;
using Marvin.Products.Samples;
using Marvin.Products.Samples.Recipe;
using Marvin.Tools;
using Marvin.Workflows;
using NUnit.Framework;

namespace Marvin.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageTest
    {
        private long _workplanId;

        private InMemoryUnitOfWorkFactory _factory;

        private const string WatchMaterial = "87654";

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();

            // prepare inmemory resource db
            _factory = new InMemoryUnitOfWorkFactory("ProductStorageTest");
            _factory.Initialize();

            // Enable test mode
            ReflectionTool.TestMode = true;

            // prepare empty workplan
            var workplan = new Workplan { Name = "TestWorkplan" };
            workplan.AddConnector("Start", NodeClassification.Start);
            workplan.AddConnector("End", NodeClassification.End);
            using (var uow = _factory.Create())
            {
                var entity = RecipeStorage.SaveWorkplan(uow, workplan);
                uow.Save();
                _workplanId = entity.Id;
            }
        }

        private static WatchProduct SetupProduct(string watchName, string identifierPrefix, short revision = 5)
        {
            var watchface = new WatchfaceProduct
            {
                Name = "Black water resistant for " + watchName,
                Identity = new ProductIdentity(identifierPrefix + "4711", revision),
                Numbers = new[] { 3, 6, 9, 12 }
            };

            var needles = new List<NeedlePartLink>
            {
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Hours needle", Identity = new ProductIdentity(identifierPrefix + "24", 1) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Minutes needle", Identity = new ProductIdentity(identifierPrefix + "1440", 2) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Seconds needle", Identity = new ProductIdentity(identifierPrefix + "B86400", 3) }
                }
            };

            var watch = new WatchProduct
            {
                Name = watchName,
                Identity = new ProductIdentity(identifierPrefix + WatchMaterial, revision),
                Watchface = new ProductPartLink<WatchfaceProduct> { Product = watchface },
                Needles = needles
            };

            return watch;
        }

        [Test]
        public void SaveWatchProduct()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var savedWatchId = watchStorage.SaveProduct(watch);

            // Assert
            using (var uow = _factory.Create())
            {
                var productEntityRepo = uow.GetRepository<IProductEntityRepository>();

                var watchEntity = productEntityRepo.GetByKey(savedWatchId);
                Assert.NotNull(watchEntity, "Failed to save or id not written");

                CheckProduct(watch, watchEntity, productEntityRepo, savedWatchId);
            }
        }

        [Test]
        public void SaveNewWatchProductVersion()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var oldWatchName = watch.Name;
            watchStorage.SaveProduct(watch);

            const string newWatchName = "Daniel Wellington";
            watch.Name = newWatchName;
            var savedWatchId = watchStorage.SaveProduct(watch);

            // Assert
            using (var uow = _factory.Create())
            {
                var productEntityRepo = uow.GetRepository<IProductEntityRepository>();

                var watchEntity = productEntityRepo.GetByKey(savedWatchId);
                Assert.NotNull(watchEntity, "Failed to save or id not written");
                Assert.AreEqual(oldWatchName, watchEntity.OldVersions.First().Name, "Old data are not equal to the previous version");
                Assert.AreEqual(newWatchName, watchEntity.CurrentVersion.Name, "Latest changes are not in the new version");

                CheckProduct(watch, watchEntity, productEntityRepo, savedWatchId);
            }
        }

        private static void CheckProduct(WatchProduct watch, ProductEntity watchEntity, IProductEntityRepository productEntityRepo, long savedWatchId)
        {
            var watchNeedlesCount = watch.Needles.Count;
            var watchEntityNeedlesCount = watchEntity.Parts.Count(p => p.Child.TypeName.Equals(nameof(NeedleProduct)));
            Assert.AreEqual(watchNeedlesCount, watchEntityNeedlesCount, "Different number of needles");

            var watchfaceEntity = watchEntity.Parts.First(p => p.Child.TypeName.Equals(nameof(WatchfaceProduct))).Child;
            Assert.NotNull(watchfaceEntity, "There is no watchface");

            var identity = (ProductIdentity)watch.Identity;
            var byIdentifier = productEntityRepo.GetByIdentity(identity.Identifier, identity.Revision);
            Assert.NotNull(byIdentifier, "New version of watch not found by identifier ");
            Assert.AreEqual(savedWatchId, byIdentifier.Id, "Different id´s");
        }

        [Test]
        public void GetWatchProduct()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var savedWatchId = watchStorage.SaveProduct(watch);
            var loadedWatch = (WatchProduct)watchStorage.LoadProduct(savedWatchId);

            // Assert
            Assert.NotNull(loadedWatch, "Failed to load from database");
            Assert.NotNull(loadedWatch.Watchface.Product.Parent, "Parent was not set");
            Assert.AreEqual(loadedWatch.Watchface.Product.Parent, loadedWatch, "Invalid parent");
            Assert.AreEqual(watch.Identity.Identifier, loadedWatch.Identity.Identifier, "Different identifier of the saved an loaded watch");
            Assert.AreEqual(watch.Watchface.Product.Identity.Identifier, loadedWatch.Watchface.Product.Identity.Identifier, "Different watchface identifier of the saved and loaded watch");
            Assert.AreEqual(watch.Needles.Count, loadedWatch.Needles.Count, "Different number of needles");
            Assert.AreEqual(watch.Watchface.Product.Numbers.Length, loadedWatch.Watchface.Product.Numbers.Length, "Different number of watch numbers");
        }

        [Test]
        public void ParentOnWatchface()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            watchStorage.SaveProduct(watch);
            var watchfaceId = watch.Watchface.Product.Id;
            var watchface = (WatchfaceProduct)watchStorage.LoadProduct(watchfaceId);

            // Assert
            Assert.NotNull(watchface, "Failed to load from database");
            Assert.NotNull(watchface.Parent, "Parent was not set");
            var watchParent = (WatchProduct)watchface.Parent;
            Assert.AreEqual(watchface, watchParent.Watchface.Product, "Product tree inconsistent!");
            Assert.AreEqual(3, watchParent.Needles.Count, "Full parent loading MUST include other part links too");
        }

        [Test]
        public void PackageOnWatch()
        {
            // Arrange
            var package = new WatchPackageProduct
            {
                Name = "Standard box",
                Identity = new ProductIdentity("9876543", 42),
                PossibleWatches = new List<ProductPartLink<WatchProduct>>
                {
                    new ProductPartLink<WatchProduct> {Product = SetupProduct("Jaques Lemans", "1")},
                    new ProductPartLink<WatchProduct> {Product = SetupProduct("Tag Heuer", "2")}
                }
            };

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            watchStorage.SaveProduct(package);
            var watchId = package.PossibleWatches[1].Product.Id;
            var watch = (WatchProduct)watchStorage.LoadProduct(watchId);

            // Assert
            Assert.NotNull(watch, "Failed to load from database");
            Assert.NotNull(watch.Parent, "Parent was not set");
            var watchParent = (WatchPackageProduct)watch.Parent;
            Assert.AreEqual(1, watchParent.PossibleWatches.Count, "Reference should only be loaded partially");
            Assert.AreEqual(watch, watchParent.PossibleWatches[0].Product, "Product tree inconsistent!");
        }

        [TestCase(true, Description = "Get the latest revision of an existing product")]
        [TestCase(false, Description = "Try to get the latest revision of a not-existing product")]
        public void LoadLatestRevision(bool exists)
        {
            const string newName = "Jaques Lemans XS";

            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            watchStorage.SaveProduct(watch);
            watch = SetupProduct(newName, string.Empty, 42);
            watchStorage.SaveProduct(watch);

            // Act
            var loadedWatch = (WatchProduct)watchStorage.LoadProduct(ProductIdentity.AsLatestRevision(exists ? WatchMaterial : "1234"));

            // Assert
            if (exists)
            {
                Assert.NotNull(loadedWatch);
                Assert.AreEqual(42, ((ProductIdentity)loadedWatch.Identity).Revision);
                Assert.AreEqual(newName, loadedWatch.Name);
            }
            else
            {
                Assert.IsNull(loadedWatch);
            }
        }

        [Test]
        public void GetProductByQuery()
        {
            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = watchStorage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            watchStorage.SaveProduct(watch);
            watch = SetupProduct("Jaques Lemans", string.Empty, 17);
            watchStorage.SaveProduct(watch);

            // Act
            var all = productMgr.GetProducts(new ProductQuery());
            var latestRevision = productMgr.GetProducts(new ProductQuery { RevisionFilter = RevisionFilter.Latest });
            var byType = productMgr.GetProducts(new ProductQuery { Type = nameof(NeedleProduct) });
            var allRevision = productMgr.GetProducts(new ProductQuery { Identifier = WatchMaterial });
            var latestByType = productMgr.GetProducts(new ProductQuery
            {
                Type = nameof(WatchProduct),
                RevisionFilter = RevisionFilter.Latest
            });
            var usages = productMgr.GetProducts(new ProductQuery
            {
                Identifier = "24",
                Selector = Selector.Parent
            });
            var needles = productMgr.GetProducts(new ProductQuery
            {
                Name = "needle",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.Greater(all.Count, latestRevision.Count);
            Assert.IsTrue(byType.All(p => p is NeedleProduct));
            Assert.IsTrue(allRevision.All(p => p.Identity.Identifier == WatchMaterial));
            Assert.GreaterOrEqual(latestByType.Count, 1);
            Assert.IsTrue(usages.All(u => u is WatchProduct));
            Assert.GreaterOrEqual(needles.Count, 3);
        }

        [Test]
        public void ShouldReturnNoProductsForWildcardInName()
        {
            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = watchStorage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            watchStorage.SaveProduct(watch);

            // Act
            var needles = productMgr.GetProducts(new ProductQuery
            {
                Name = "*needle",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.GreaterOrEqual(needles.Count, 0, "There should be no products if a wildcard was used for the name");
        }

        [Test]
        public void IdentifierQueryShouldNotBeCaseSensitive()
        {
            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = watchStorage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            watchStorage.SaveProduct(watch);

            // Act
            var products = productMgr.GetProducts(new ProductQuery
            {
                Identifier = "b*",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.AreEqual(1, products.Count, "There should be a product for the given query");
        }

        [TestCase(false, false, Description = "Duplicate product with valid id")]
        [TestCase(false, true, Description = "Duplicate product, but identity already taken")]
        [TestCase(true, false, Description = "Duplicate product but with template missmatch")]
        public void DuplicateProduct(bool crossTypeIdentifier, bool revisionTaken)
        {
            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = watchStorage
            };
            productMgr.ProductChanged += (sender, product) => { };
            var watch = SetupProduct("Jaques Lemans", "321");
            watchStorage.SaveProduct(watch);
            var recipe = new WatchProductRecipe
            {
                Product = watch,
                Classification = RecipeClassification.Default,
                Name = "TestRecipe",
                Workplan = new Workplan { Id = _workplanId }
            };
            watchStorage.SaveRecipe(recipe);

            // Act (& Assert)
            WatchProduct duplicate = null;
            if (crossTypeIdentifier | revisionTaken)
            {
                var newIdentity = crossTypeIdentifier
                    ? new ProductIdentity("3214711", 7)
                    : new ProductIdentity("321" + WatchMaterial, 5);
                var ex = Assert.Throws<IdentityConflictException>(() =>
                {
                    duplicate = (WatchProduct)productMgr.Duplicate(watch.Id, newIdentity);
                });
                Assert.AreEqual(crossTypeIdentifier, ex.InvalidTemplate);
                return;
            }
            else
            {
                Assert.DoesNotThrow(() =>
                {
                    duplicate = (WatchProduct)productMgr.Duplicate(watch.Id,
                        new ProductIdentity("654" + WatchMaterial, 1));
                });
            }

            var recipeDuplicates = watchStorage.LoadRecipes(duplicate.Id, RecipeClassification.CloneFilter);

            // Assert
            Assert.AreEqual(watch.Watchface.Product.Id, duplicate.Watchface.Product.Id);
            Assert.AreEqual(watch.Needles.Sum(n => n.Product.Id), duplicate.Needles.Sum(n => n.Product.Id));
            Assert.Greater(recipeDuplicates.Count, 0);
            Assert.AreNotEqual(recipe.Id, recipeDuplicates[0].Id);
            Assert.AreEqual(recipe.Name, recipeDuplicates[0].Name);
            Assert.AreEqual(recipe.Classification, recipeDuplicates[0].Classification);
        }

        [TestCase(true, Description = "Remove a product that is still used")]
        [TestCase(false, Description = "Remove a product that is not used")]
        public void RemoveProduct(bool stillUsed)
        {
            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = watchStorage
            };
            var watch = SetupProduct("Jaques Lemans", "567");
            watchStorage.SaveProduct(watch);

            // Act
            bool result;
            if (stillUsed)
                result = productMgr.DeleteProduct(watch.Watchface.Product.Id);
            else
                result = productMgr.DeleteProduct(watch.Id);

            // Assert
            Assert.AreEqual(stillUsed, !result);
            if (stillUsed)
                return;

            var matches = productMgr.GetProducts(new ProductQuery
            {
                RevisionFilter = RevisionFilter.Specific,
                Revision = 5,
                Identifier = watch.Identity.Identifier
            });
            Assert.AreEqual(0, matches.Count);
        }
    }
}
