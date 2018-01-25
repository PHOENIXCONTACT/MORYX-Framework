using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Products.IntegrationTests.UnitOfWorkFactories;
using Marvin.Products.Model;
using Marvin.Products.Samples;
using Marvin.Products.Samples.Model;
using NUnit.Framework;
using OperatingSystem = Marvin.Products.Samples.Model.OperatingSystem;

namespace Marvin.Products.IntegrationTests
{
    [TestFixture]
    public class ArticleStorageTests
    {
        private InMemoryUnitOfWorkFactory _factory;
        private WatchProductStorage _storage;
        private readonly IDictionary<string, long[]> _ids = new Dictionary<string, long[]>();

        [OneTimeSetUp]
        public void CreateDb()
        {
            // prepare inmemory resource db
            _factory = new InMemoryUnitOfWorkFactory("ArticleStorageTest");
            _factory.Initialize();

            using (var uow = _factory.Create())
            {
                var prodRepo = uow.GetRepository<IProductEntityRepository>();
                var propertiesRepo = uow.GetRepository<ISmartWatchProductPropertiesEntityRepository>();
                var linkRepo = uow.GetRepository<IPartLinkRepository>();

                // Create products
                var watchProduct = prodRepo.Create("1234", 1, nameof(WatchProduct));
                var watchProperties = propertiesRepo.Create("Cool Watch");
                watchProperties.OperatingSystem = OperatingSystem.Windows2012Server;
                watchProduct.SetCurrentVersion(watchProperties);

                var watchfaceProduct = prodRepo.Create("5678", 2, nameof(WatchFaceProduct));
                var watchfaceProperties = propertiesRepo.Create("Cool watchface");
                watchfaceProduct.SetCurrentVersion(watchfaceProperties);

                var hourProduct = prodRepo.Create("567871", 3, nameof(NeedleProduct));
                var hourProperties = propertiesRepo.Create("Hour");
                hourProduct.SetCurrentVersion(hourProperties);

                var secondsProduct = prodRepo.Create("3455644", 3, nameof(NeedleProduct));
                var secondsProperties = propertiesRepo.Create("Seconds");
                secondsProduct.SetCurrentVersion(secondsProperties);

                // Create part links
                var singleLink = linkRepo.Create(nameof(WatchProduct.Watchface));
                singleLink.Parent = watchProduct;
                singleLink.Child = watchfaceProduct;

                var multiLink1 = linkRepo.Create(nameof(WatchProduct.Needles));
                multiLink1.Parent = watchProduct;
                multiLink1.Child = hourProduct;

                var multiLink2 = linkRepo.Create(nameof(WatchProduct.Needles));
                multiLink2.Parent = watchProduct;
                multiLink2.Child = secondsProduct;

                uow.Save();

                _ids["Root"] = new[] { watchProduct.Id };
                _ids["Part"] = new[] { watchfaceProduct.Id };
                _ids["SingleChild"] = new[] { singleLink.Id };
                _ids["Children"] = new[] { multiLink1.Id, multiLink2.Id };
            }

            _storage = new WatchProductStorage { Factory = _factory };
        }

        [Test]
        public void SaveArticle()
        {
            // Arrange
            var product = _storage.LoadProduct(_ids["Root"][0]);
            var article = product.CreateInstance();

            // Act
            _storage.SaveArticles(new[] { article });

            // Assert
            using (var uow = _factory.Create())
            {
                var root = uow.GetRepository<IArticleEntityRepository>().GetByKey(article.Id);
                Assert.NotNull(root, "Failed to save or id not written");

                var parts = root.Parts;
                Assert.AreEqual(1, parts.Count, "Invalid number of parts!"); // needles will be skipped for saving

                var single = parts.FirstOrDefault(p => p.PartLinkId == _ids["SingleChild"][0]);
                Assert.NotNull(single, "Single part not saved!");
                Assert.AreEqual(0, parts.Count(p => p.PartLinkId == _ids["Children"][0]), "Collection not saved");
                Assert.AreEqual(0, parts.Count(p => p.PartLinkId == _ids["Children"][1]), "Collection not saved");
            }
        }

        [Test]
        public void GetArticle()
        {
            // Arrange
            long rootId;
            using (var uow = _factory.Create())
            {
                var repo = uow.GetRepository<IArticleEntityRepository>();

                // Root article
                var root = repo.Create();
                root.ProductId = _ids["Root"][0];
                root.Identifier = "567";

                var binaryDate = DateTime.MinValue.ToBinary();
                root.ExtensionData = binaryDate.ToString("X");

                // Single part and sub part
                CreatePart(repo, root, _ids["SingleChild"][0]);

                uow.Save();

                rootId = root.Id;
            }

            // Act
            var article = (WatchArticle)_storage.LoadArticle(rootId);

            // Assert
            Assert.NotNull(article, "Failed to load from db!");
            Assert.IsInstanceOf<WatchFaceArticle>(article.WatchFace, "Failed to fetch single part");
            Assert.AreEqual(3, ((IArticleParts)article).Parts.Count, "Invalid number of parts fetched!");
        }

        private void CreatePart(IArticleEntityRepository repo, ArticleEntity parent, long linkId)
        {
            var part = repo.Create();
            part.ProductId = _ids["Part"][0];
            part.Parent = parent;
            part.PartLinkId = linkId;
        }
    }
}
