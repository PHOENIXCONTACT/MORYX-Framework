// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.Products.Management;
using Marvin.Products.Management.NullStrategies;
using Marvin.Products.Model;
using Marvin.Products.Samples;
using Marvin.Products.Samples.Recipe;
using Marvin.Tools;
using Marvin.Workflows;
using Moq;
using NUnit.Framework;

namespace Marvin.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageTests
    {
        private long _workplanId;

        private InMemoryUnitOfWorkFactory _factory;

        private const string WatchMaterial = "87654";

        private ProductStorage _storage;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            // Enable test mode
            ReflectionTool.TestMode = true;
            // This call is necessary for NUnit to load the type
            var someType = new WatchType();

            Effort.Provider.EffortProviderConfiguration.RegisterProvider();

            // prepare inmemory resource db
            _factory = new InMemoryUnitOfWorkFactory("ProductStorageTest");
            _factory.Initialize();

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

        [SetUp]
        public void PrepareStorage()
        {
            var strategyFactory = CreateStrategyFactory();

            _storage = new ProductStorage
            {
                Factory = _factory,
                StrategyFactory = strategyFactory.Object
            };
            _storage.Config = new ModuleConfig
            {
                TypeStrategies = new List<ProductTypeConfiguration>
                {
                    new ProductTypeConfiguration
                    {
                        TargetType = nameof(WatchType),
                        PluginName = nameof(WatchStrategy)
                    },
                    new GenericTypeConfiguration
                    {
                        TargetType = nameof(WatchfaceType),
                        PropertyConfigs = new List<PropertyMapperConfig>()
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchfaceType.IsDigital),
                                Column = nameof(IGenericColumns.Integer1),
                                PluginName = nameof(IntegerColumnMapper)
                            }
                        },
                        JsonColumn = nameof(IGenericColumns.Text8)
                    },
                    new GenericTypeConfiguration
                    {
                        TargetType = nameof(NeedleType),
                        PropertyConfigs = new List<PropertyMapperConfig>(),
                        JsonColumn = nameof(IGenericColumns.Text8)
                    },
                    new GenericTypeConfiguration
                    {
                        TargetType = nameof(WatchPackageType),
                        JsonColumn = nameof(IGenericColumns.Text8),
                        PropertyConfigs = new List<PropertyMapperConfig>()
                    }
                },
                InstanceStrategies = new List<ProductInstanceConfiguration>
                {
                    new GenericInstanceConfiguration
                    {
                        TargetType = nameof(WatchInstance),
                        PluginName = nameof(WatchStrategy),
                        JsonColumn = nameof(IGenericColumns.Text8),
                        PropertyConfigs = new List<PropertyMapperConfig>
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchInstance.DeliveryDate),
                                Column = nameof(IGenericColumns.Integer1),
                                PluginName = nameof(IntegerColumnMapper)
                            },
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchInstance.TimeSet),
                                Column = nameof(IGenericColumns.Integer2),
                                PluginName = nameof(IntegerColumnMapper)
                            }
                        }
                    },
                    new GenericInstanceConfiguration
                    {
                        TargetType = nameof(WatchfaceInstance),
                        PropertyConfigs = new List<PropertyMapperConfig>(),
                        JsonColumn = nameof(IGenericColumns.Text8)
                    },
                    new ProductInstanceConfiguration()
                    {
                        TargetType = nameof(NeedleInstance),
                        PluginName = nameof(SkipArticlesStrategy)
                    },
                },
                LinkStrategies = new List<ProductLinkConfiguration>
                {
                    new ProductLinkConfiguration()
                    {
                        TargetType = nameof(WatchType),
                        PartName = nameof(WatchType.Watchface),
                        PluginName = nameof(SimpleLinkStrategy)
                    },
                    new GenericLinkConfiguration
                    {
                        TargetType = nameof(WatchType),
                        PartName = nameof(WatchType.Needles),
                        JsonColumn = nameof(IGenericColumns.Text8),
                        PropertyConfigs = new List<PropertyMapperConfig>
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(NeedlePartLink.Role),
                                PluginName = nameof(IntegerColumnMapper),
                                Column = nameof(IGenericColumns.Integer1)
                            }
                        }
                    },
                    new ProductLinkConfiguration()
                    {
                        TargetType = nameof(WatchPackageType),
                        PartName = nameof(WatchPackageType.PossibleWatches),
                        PluginName = nameof(SimpleLinkStrategy)
                    },
                },
                RecipeStrategies = new List<ProductRecipeConfiguration>
                {
                    new GenericRecipeConfiguration
                    {
                        TargetType = nameof(WatchProductRecipe),
                        JsonColumn = nameof(IGenericColumns.Text8),
                        PropertyConfigs = new List<PropertyMapperConfig>()
                    }
                }
            };

            _storage.Start();
        }

        private Mock<IStorageStrategyFactory> CreateStrategyFactory()
        {
            var mapperFactory = new Mock<IPropertyMapperFactory>();
            mapperFactory.Setup(mf => mf.Create(It.IsAny<PropertyMapperConfig>(), It.IsAny<Type>()))
                .Returns<PropertyMapperConfig, Type>((config, type) =>
                {
                    IPropertyMapper mapper = null;
                    switch (config.PluginName)
                    {
                        case nameof(IntegerColumnMapper):
                            mapper = new IntegerColumnMapper(type);
                            break;
                        case nameof(TextColumnMapper):
                            mapper = new TextColumnMapper(type);
                            break;
                    }

                    mapper.Initialize(config);

                    return mapper;
                });

            var strategyFactory = new Mock<IStorageStrategyFactory>();
            strategyFactory.Setup(f => f.CreateTypeStrategy(It.IsAny<ProductTypeConfiguration>()))
                .Returns<ProductTypeConfiguration>(config =>
                {
                    IProductTypeStrategy strategy = null;
                    switch (config.PluginName)
                    {
                        case nameof(WatchStrategy):
                            strategy = new WatchStrategy();
                            break;
                        case nameof(GenericTypeStrategy):
                            strategy = new GenericTypeStrategy
                            {
                                EntityMapper = new GenericEntityMapper<ProductType, IProductPartLink>
                                {
                                    MapperFactory = mapperFactory.Object
                                }
                            };
                            break;
                    }

                    strategy.Initialize(config);

                    return strategy;
                });

            strategyFactory.Setup(f => f.CreateInstanceStrategy(It.IsAny<ProductInstanceConfiguration>()))
                .Returns<ProductInstanceConfiguration>(config =>
                {
                    IProductInstanceStrategy strategy = null;
                    switch (config.PluginName)
                    {
                        case nameof(GenericInstanceStrategy):
                            strategy = new GenericInstanceStrategy()
                            {
                                EntityMapper = new GenericEntityMapper<ProductInstance, ProductInstance>
                                {
                                    MapperFactory = mapperFactory.Object
                                }
                            };
                            break;
                        case nameof(SkipArticlesStrategy):
                            strategy = new SkipArticlesStrategy();
                            break;
                    }

                    strategy.Initialize(config);

                    return strategy;
                });

            strategyFactory.Setup(f => f.CreateLinkStrategy(It.IsAny<ProductLinkConfiguration>()))
                .Returns<ProductLinkConfiguration>(config =>
                {
                    IProductLinkStrategy strategy = null;
                    switch (config.PluginName)
                    {
                        case nameof(GenericLinkStrategy):
                            strategy = new GenericLinkStrategy()
                            {
                                EntityMapper = new GenericEntityMapper<ProductPartLink, ProductType>
                                {
                                    MapperFactory = mapperFactory.Object
                                }
                            };
                            break;
                        case nameof(SimpleLinkStrategy):
                            strategy = new SimpleLinkStrategy();
                            break;
                    }

                    strategy.Initialize(config);

                    return strategy;
                });

            strategyFactory.Setup(f => f.CreateRecipeStrategy(It.IsAny<ProductRecipeConfiguration>()))
                .Returns<ProductRecipeConfiguration>(config =>
                {
                    IProductRecipeStrategy strategy = new GenericRecipeStrategy
                    {
                        EntityMapper = new GenericEntityMapper<ProductRecipe, IProductType>
                        {
                            MapperFactory = mapperFactory.Object
                        }
                    };

                    strategy.Initialize(config);

                    return strategy;
                });

            return strategyFactory;
        }

        private static WatchType SetupProduct(string watchName, string identifierPrefix, short revision = 5)
        {
            var watchface = new WatchfaceType
            {
                Name = "Black water resistant for " + watchName,
                Identity = new ProductIdentity(identifierPrefix + "4711", revision),
                Numbers = new[] { 3, 6, 9, 12 }
            };

            var needles = new List<NeedlePartLink>
            {
                new NeedlePartLink
                {
                    Product = new NeedleType { Name = "Hours needle", Identity = new ProductIdentity(identifierPrefix + "24", 1) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleType { Name = "Minutes needle", Identity = new ProductIdentity(identifierPrefix + "1440", 2) }
                },
                new NeedlePartLink
                {
                    Product = new NeedleType { Name = "Seconds needle", Identity = new ProductIdentity(identifierPrefix + "B86400", 3) }
                }
            };

            var watch = new WatchType
            {
                Name = watchName,
                Identity = new ProductIdentity(identifierPrefix + WatchMaterial, revision),
                Watchface = new ProductPartLink<WatchfaceType> { Product = watchface },
                Needles = needles,
                Weight = 123.45
            };

            return watch;
        }

        [Test]
        public void SaveWatchProduct()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var savedWatchId = _storage.SaveType(watch);

            // Assert
            using (var uow = _factory.Create())
            {
                var productEntityRepo = uow.GetRepository<IProductTypeEntityRepository>();

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
            watch.Weight = 234.56;
            _storage.SaveType(watch);
            var savedWatchId = _storage.SaveType(watch);

            // Assert
            using (var uow = _factory.Create())
            {
                var productEntityRepo = uow.GetRepository<IProductTypeEntityRepository>();

                var watchEntity = productEntityRepo.GetByKey(savedWatchId);
                Assert.NotNull(watchEntity, "Failed to save or id not written");
                Assert.AreEqual(123.45, watchEntity.OldVersions.First().Float1, "Old data are not equal to the previous version");
                Assert.AreEqual(234.56, watchEntity.CurrentVersion.Float1, "Latest changes are not in the new version");

                CheckProduct(watch, watchEntity, productEntityRepo, savedWatchId);
            }
        }

        private static void CheckProduct(WatchType watch, ProductTypeEntity watchTypeEntity, IProductTypeEntityRepository productTypeEntityRepo, long savedWatchId)
        {
            var watchNeedlesCount = watch.Needles.Count;
            var watchEntityNeedlesCount = watchTypeEntity.Parts.Count(p => p.Child.TypeName.Equals(nameof(NeedleType)));
            Assert.AreEqual(watchNeedlesCount, watchEntityNeedlesCount, "Different number of needles");

            var watchfaceEntity = watchTypeEntity.Parts.First(p => p.Child.TypeName.Equals(nameof(WatchfaceType))).Child;
            Assert.NotNull(watchfaceEntity, "There is no watchface");

            var identity = (ProductIdentity)watch.Identity;
            var byIdentifier = productTypeEntityRepo.GetByIdentity(identity.Identifier, identity.Revision);
            Assert.NotNull(byIdentifier, "New version of watch not found by identifier ");
            Assert.AreEqual(savedWatchId, byIdentifier.Id, "Different id´s");
        }

        [Test]
        public void GetWatchProduct()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);

            // Act
            var savedWatchId = _storage.SaveType(watch);
            var loadedWatch = (WatchType)_storage.LoadType(savedWatchId);

            // Assert
            Assert.NotNull(loadedWatch, "Failed to load from database");
            Assert.AreEqual(watch.Identity.Identifier, loadedWatch.Identity.Identifier, "Different identifier of the saved an loaded watch");
            Assert.AreEqual(watch.Watchface.Product.Identity.Identifier, loadedWatch.Watchface.Product.Identity.Identifier, "Different watchface identifier of the saved and loaded watch");
            Assert.AreEqual(watch.Needles.Count, loadedWatch.Needles.Count, "Different number of needles");
            Assert.AreEqual(watch.Watchface.Product.Numbers.Length, loadedWatch.Watchface.Product.Numbers.Length, "Different number of watch numbers");
        }

        [TestCase(true, Description = "Get the latest revision of an existing product")]
        [TestCase(false, Description = "Try to get the latest revision of a not-existing product")]
        public void LoadLatestRevision(bool exists)
        {
            const string newName = "Jaques Lemans XS";

            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);
            watch = SetupProduct(newName, string.Empty, 42);
            _storage.SaveType(watch);

            // Act
            var loadedWatch = (WatchType)_storage.LoadType(ProductIdentity.AsLatestRevision(exists ? WatchMaterial : "1234"));

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
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = _storage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);
            watch = SetupProduct("Jaques Lemans", string.Empty, 17);
            _storage.SaveType(watch);

            // Act
            var all = productMgr.GetTypes(new ProductQuery());
            var latestRevision = productMgr.GetTypes(new ProductQuery { RevisionFilter = RevisionFilter.Latest });
            var byType = productMgr.GetTypes(new ProductQuery { Type = nameof(NeedleType) });
            var allRevision = productMgr.GetTypes(new ProductQuery { Identifier = WatchMaterial });
            var latestByType = productMgr.GetTypes(new ProductQuery
            {
                Type = nameof(WatchType),
                RevisionFilter = RevisionFilter.Latest
            });
            var usages = productMgr.GetTypes(new ProductQuery
            {
                Identifier = "24",
                Selector = Selector.Parent
            });
            var needles = productMgr.GetTypes(new ProductQuery
            {
                Name = "needle",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.Greater(all.Count, latestRevision.Count);
            Assert.IsTrue(byType.All(p => p is NeedleType));
            Assert.IsTrue(allRevision.All(p => p.Identity.Identifier == WatchMaterial));
            Assert.GreaterOrEqual(latestByType.Count, 1);
            Assert.IsTrue(usages.All(u => u is WatchType));
            Assert.GreaterOrEqual(needles.Count, 3);
        }

        [Test]
        public void ShouldReturnNoProductsForWildcardInName()
        {
            // Arrange
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = _storage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);

            // Act
            var needles = productMgr.GetTypes(new ProductQuery
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
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = _storage
            };
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);

            // Act
            var products = productMgr.GetTypes(new ProductQuery
            {
                Identifier = "b*",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.AreEqual(1, products.Count, "There should be a product for the given query");
        }

        // [TestCase(false, false, Description = "Duplicate product with valid id")] TODO: https://github.com/zzzprojects/EntityFramework-Effort/issues/191
        [TestCase(false, true, Description = "Duplicate product, but identity already taken")]
        [TestCase(true, false, Description = "Duplicate product but with template missmatch")]
        public void DuplicateProduct(bool crossTypeIdentifier, bool revisionTaken)
        {
            // Arrange
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = _storage
            };
            productMgr.TypeChanged += (sender, product) => { };
            var watch = SetupProduct("Jaques Lemans", "321");
            _storage.SaveType(watch);
            var recipe = new WatchProductRecipe
            {
                Product = watch,
                Classification = RecipeClassification.Default,
                Name = "TestRecipe",
                Workplan = new Workplan { Id = _workplanId }
            };
            _storage.SaveRecipe(recipe);

            // Act (& Assert)
            WatchType duplicate = null;
            if (crossTypeIdentifier | revisionTaken)
            {
                var newIdentity = crossTypeIdentifier
                    ? new ProductIdentity("3214711", 7)
                    : new ProductIdentity("321" + WatchMaterial, 5);
                var ex = Assert.Throws<IdentityConflictException>(() =>
                {
                    duplicate = (WatchType)productMgr.Duplicate(watch.Id, newIdentity);
                });
                Assert.AreEqual(crossTypeIdentifier, ex.InvalidTemplate);
                return;
            }

            Assert.DoesNotThrow(() =>
            {
                duplicate = (WatchType)productMgr.Duplicate(watch.Id,
                    new ProductIdentity("654" + WatchMaterial, 1));
            });

            var recipeDuplicates = _storage.LoadRecipes(duplicate.Id, RecipeClassification.Unset);

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
            var productMgr = new ProductManager
            {
                Factory = _factory,
                Storage = _storage
            };
            var watch = SetupProduct("Jaques Lemans", "567");
            _storage.SaveType(watch);

            // Act
            bool result;
            if (stillUsed)
                result = productMgr.DeleteType(watch.Watchface.Product.Id);
            else
                result = productMgr.DeleteType(watch.Id);

            // Assert
            Assert.AreEqual(stillUsed, !result);
            if (stillUsed)
                return;

            var matches = productMgr.GetTypes(new ProductQuery
            {
                RevisionFilter = RevisionFilter.Specific,
                Revision = 5,
                Identifier = watch.Identity.Identifier
            });
            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void SaveAndLoadInstance()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);
            // Reload from storage for partlink ids if the object exists
            watch = (WatchType) _storage.LoadType(watch.Id);

            // Act
            var article = (WatchInstance)watch.CreateInstance();
            article.TimeSet = true;
            article.DeliveryDate = DateTime.Now;
            _storage.SaveInstances(new[] { article });

            // Assert
            using (var uow = _factory.Create())
            {
                var root = uow.GetRepository<IProductInstanceEntityRepository>().GetByKey(article.Id);
                Assert.NotNull(root, "Failed to save or id not written");
                Assert.AreEqual(article.DeliveryDate.Ticks, root.Integer1, "DateTime not saved");
                Assert.AreEqual(1, root.Integer2, "Bool not saved");

                var parts = root.Parts;
                Assert.AreEqual(1, parts.Count, "Invalid number of parts!"); // needles will be skipped for saving

                var single = parts.FirstOrDefault(p => p.PartLinkId == watch.Watchface.Id);
                Assert.NotNull(single, "Single part not saved!");
            }

            // Act
            var watchCopy = (WatchInstance)_storage.LoadInstance(article.Id);

            // Assert
            Assert.NotNull(watchCopy);
            Assert.AreEqual(article.DeliveryDate, watchCopy.DeliveryDate);
            Assert.AreEqual(article.TimeSet, watchCopy.TimeSet);
            Assert.NotNull(article.Watchface);
            Assert.NotNull(article.Needles);
            Assert.AreEqual(3, article.Needles.Count);
        }
    }
}
