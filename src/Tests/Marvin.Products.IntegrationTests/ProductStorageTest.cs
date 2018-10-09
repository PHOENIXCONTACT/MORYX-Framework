using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Products.Model;
using Marvin.Products.Samples;
using NUnit.Framework;

namespace Marvin.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageTest
    {
        private InMemoryUnitOfWorkFactory _factory;

        private const string WatchMaterial = "87654";

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();

            // prepare inmemory resource db
            _factory = new InMemoryUnitOfWorkFactory("ProductStorageTest");
            _factory.Initialize();
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
                    Product = new NeedleProduct { Name = "Seconds needle", Identity = new ProductIdentity(identifierPrefix + "86400", 3) }
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

        [Test]
        public void LoadLatestRevision()
        {
            const string newName = "Jaques Lemans XS";

            // Arrange
            var watchStorage = new WatchProductStorage { Factory = _factory };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            watchStorage.SaveProduct(watch);
            watch = SetupProduct(newName, string.Empty, 42);
            watchStorage.SaveProduct(watch);

            // Act
            var loadedWatch = (WatchProduct) watchStorage.LoadProduct(ProductIdentity.AsLatestRevision(WatchMaterial));

            // Assert
            Assert.NotNull(loadedWatch);
            Assert.AreEqual(42, ((ProductIdentity)loadedWatch.Identity).Revision);
            Assert.AreEqual(newName, loadedWatch.Name);
        }
    }
}
