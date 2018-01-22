using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Model;
using Marvin.Products.Model;
using Marvin.Products.Samples;
using NUnit.Framework;

namespace Marvin.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageTest
    {
        private InMemoryUnitOfWorkFactory _factory;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());

            // prepare inmemory resource db
            _factory = new InMemoryUnitOfWorkFactory();
            _factory.Initialize();
        }

        private static WatchProduct SetupProduct()
        {
            var watchface = new WatchfaceProduct
            {
                Name = "Black water resistant",
                Identity = new ProductIdentity("4711", 1),
                Numbers = new []{3, 6, 9, 12}
            };

            var needles = new List<NeedlePartLink>
            {
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Hours needle", Identity = new ProductIdentity("24", 1) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Minutes needle", Identity = new ProductIdentity("1440", 2) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleProduct { Name = "Seconds needle", Identity = new ProductIdentity("86400", 3) }
                }
            };

            var watch = new WatchProduct
            {
                Name = "Jaques Lemans",
                Identity = new ProductIdentity("87654", 5),
                Watchface = new ProductPartLink<WatchfaceProduct>{Product = watchface},
                Needles = needles
            };

            return watch;
        }

        [Test]
        public void SaveWatchProduct()
        {
            // Arrange
            var watch = SetupProduct();

            // Act
            var watchStorage = new WatchProductStorage {Factory = _factory};
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
            var watch = SetupProduct();

            // Act
            var watchStorage = new WatchProductStorage {Factory = _factory};
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

            var identity = (ProductIdentity) watch.Identity;
            var byIdentifier = productEntityRepo.GetByIdentity(identity.Identifier, identity.Revision);
            Assert.NotNull(byIdentifier, "New version of watch not found by identifier ");
            Assert.AreEqual(savedWatchId, byIdentifier.Id, "Different id´s");
        }

        [Test]
        public void GetWatchProduct()
        {
            // Arrange
            var watch = SetupProduct();

            // Act
            var watchStorage = new WatchProductStorage {Factory = _factory};
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
            var watch = SetupProduct();

            // Act
            var watchStorage = new WatchProductStorage { Factory = _factory };
            watchStorage.SaveProduct(watch);
            var watchfaceId = watch.Watchface.Product.Id;
            var watchface = (WatchfaceProduct)watchStorage.LoadProduct(watchfaceId);

            // Assert
            Assert.NotNull(watchface, "Failed to load from database");
            Assert.NotNull(watchface.Parent, "Parent was not set");
            var watchParent = (WatchProduct) watchface.Parent;
            Assert.AreEqual(watchface, watchParent.Watchface.Product, "Product tree inconsistent!");
        }
    }
}
