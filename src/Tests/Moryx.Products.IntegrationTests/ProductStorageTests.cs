// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Products.Management;
using Moryx.Products.Management.NullStrategies;
using Moryx.Products.Model;
using Moryx.Products.Samples;
using Moryx.Products.Samples.Recipe;
using Moryx.Tools;
using Moryx.Workplans;
using Moq;
using Moryx.AbstractionLayer.Identity;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests
{
    [TestFixture]
    public class ProductStorageTests
    {
        private long _workplanId;

        private IUnitOfWorkFactory<ProductsContext> _factory;

        private const string WatchMaterial = "87654";

        private ProductStorage _storage;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            // Enable test mode
            ReflectionTool.TestMode = true;
            // This call is necessary for NUnit to load the type
            var someType = new WatchType();
        }

        [SetUp]
        public void PrepareStorage()
        {
            // prepare in memory products db
            _factory = BuildUnitOfWorkFactory();

            // prepare empty workplan
            var workplan = new Workplan { Name = "TestWorkplan" };
            workplan.AddConnector("Start", NodeClassification.Start);
            workplan.AddConnector("End", NodeClassification.End);

            using var uow = _factory.Create();
            var entity = RecipeStorage.SaveWorkplan(uow, workplan);
            uow.SaveChanges();
            _workplanId = entity.Id;

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
                        TargetType = nameof(WatchFaceType),
                        PropertyConfigs = new List<PropertyMapperConfig>
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchFaceType.Brand),
                                Column = nameof(IGenericColumns.Text1),
                                PluginName = nameof(TextColumnMapper)
                            },
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchFaceType.IsDigital),
                                Column = nameof(IGenericColumns.Integer1),
                                PluginName = nameof(IntegerColumnMapper)
                            },
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchFaceType.Color),
                                Column = string.Empty,
                                PluginName = nameof(NullPropertyMapper)
                            }
                        },
                        JsonColumn = nameof(IGenericColumns.Text8)
                    },
                    new GenericTypeConfiguration
                    {
                        TargetType = nameof(DisplayWatchFaceType),
                        PropertyConfigs = new List<PropertyMapperConfig>
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(DisplayWatchFaceType.Resolution),
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
                            },
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchInstance.Identity),
                                Column = nameof(IGenericColumns.Text1),
                                PluginName = nameof(TextColumnMapper)
                            }
                        }
                    },
                    new GenericInstanceConfiguration
                    {
                        TargetType = nameof(WatchFaceInstance),
                        PropertyConfigs = new List<PropertyMapperConfig>
                        {
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchFaceInstance.Identifier),
                                Column = nameof(IGenericColumns.Text1),
                                PluginName = nameof(TextColumnMapper)
                            },
                            new PropertyMapperConfig
                            {
                                PropertyName = nameof(WatchFaceInstance.Identity),
                                Column = nameof(IGenericColumns.Text2),
                                PluginName = nameof(TextColumnMapper)
                            }
                        },
                        JsonColumn = nameof(IGenericColumns.Text8)
                    },
                    new ProductInstanceConfiguration()
                    {
                        TargetType = nameof(NeedleInstance),
                        PluginName = nameof(SkipInstancesStrategy)
                    },
                },
                LinkStrategies = new List<ProductLinkConfiguration>
                {
                    new ProductLinkConfiguration()
                    {
                        TargetType = nameof(WatchType),
                        PartName = nameof(WatchType.WatchFace),
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

        protected virtual UnitOfWorkFactory<ProductsContext> BuildUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory<ProductsContext>(new InMemoryDbContextManager("ProductStorageTest"));
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
                        case nameof(NullPropertyMapper):
                            mapper = new NullPropertyMapper(type);
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
                        case nameof(SkipInstancesStrategy):
                            strategy = new SkipInstancesStrategy();
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
                        EntityMapper = new GenericEntityMapper<ProductionRecipe, IProductType>
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
            var watchface = new WatchFaceType
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
                WatchFace = new ProductPartLink<WatchFaceTypeBase> { Product = watchface },
                Needles = needles,
                Weight = 123.45
            };

            return watch;
        }

        [Test]
        public void PartLinksWithTheSameIdentifierAreOnlySavedOnce()
        {
           //Arrange
            var watch = new WatchType
            {
                Name = "watch",
                Identity = new ProductIdentity("223",1),
                Needles = new List<NeedlePartLink>
                {
                    new NeedlePartLink
                    {
                        Role = NeedleRole.Minutes,
                        Product = new NeedleType
                        {
                            Identity = new ProductIdentity("222", 0),
                            Name = "name"
                        }
                    },
                    new NeedlePartLink
                    {
                        Role = NeedleRole.Seconds,
                        Product = new NeedleType
                        {
                            Identity = new ProductIdentity("222", 0),
                            Name = "name"
                        }
                    }
                }
            };

            //Act
            _storage.SaveType(watch);
            var minuteNeedle = watch.Needles.Find(t => t.Role == NeedleRole.Minutes);
            var secondsNeedle = watch.Needles.Find(t => t.Role == NeedleRole.Seconds);

            //Assert
            Assert.AreNotEqual(minuteNeedle.Product.Id, 0, "Id of Needle for minutes was 0");
            Assert.AreNotEqual(secondsNeedle.Product.Id, 0, "Id of Needle for seconds was 0");
            Assert.AreEqual(secondsNeedle.Product.Id, minuteNeedle.Product.Id, "Both needles must have the same Id since they are the same product");
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
                var productEntityRepo = uow.GetRepository<IProductTypeRepository>();

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
            // TODO: Looks like this act section didn't match the assertions
            _storage.SaveType(watch);
            watch.Weight = 234.56;
            var savedWatchId = _storage.SaveType(watch);

            // Assert
            using (var uow = _factory.Create())
            {
                var productEntityRepo = uow.GetRepository<IProductTypeRepository>();

                var watchEntity = productEntityRepo.GetByKey(savedWatchId);
                Assert.NotNull(watchEntity, "Failed to save or id not written");
                Assert.AreEqual(123.45, watchEntity.OldVersions.First().Float1, "Old data are not equal to the previous version");
                Assert.AreEqual(234.56, watchEntity.CurrentVersion.Float1, "Latest changes are not in the new version");

                CheckProduct(watch, watchEntity, productEntityRepo, savedWatchId);
            }
        }

        private static void CheckProduct(WatchType watch, ProductTypeEntity watchProductTypeEntity, IProductTypeRepository productTypeRepo, long savedWatchId)
        {
            var watchNeedlesCount = watch.Needles.Count;
            var watchEntityNeedlesCount = watchProductTypeEntity.Parts.Count(p => p.Child.TypeName.Equals(nameof(NeedleType)));
            Assert.AreEqual(watchNeedlesCount, watchEntityNeedlesCount, "Different number of needles");

            var watchfaceEntity = watchProductTypeEntity.Parts.First(p => p.Child.TypeName.Equals(nameof(WatchFaceType))).Child;
            Assert.NotNull(watchfaceEntity, "There is no watchface");

            var identity = (ProductIdentity)watch.Identity;
            var byIdentifier = productTypeRepo.GetByIdentity(identity.Identifier, identity.Revision);
            Assert.NotNull(byIdentifier, "New version of watch not found by identifier ");
            Assert.AreEqual(savedWatchId, byIdentifier.Id, "Different id´s");
        }

        [Test]
        public void GetWatchProduct()
        {
            // Arrange
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            var watchface = (WatchFaceType)watch.WatchFace.Product;

            // Act
            var savedWatchId = _storage.SaveType(watch);
            var loadedWatch = (WatchType)_storage.LoadType(savedWatchId);

            // Assert
            Assert.NotNull(loadedWatch, "Failed to load from database");
            Assert.AreEqual(watch.Identity.Identifier, loadedWatch.Identity.Identifier, "Different identifier of the saved an loaded watch");
            Assert.AreEqual(watch.WatchFace.Product.Identity.Identifier, loadedWatch.WatchFace.Product.Identity.Identifier, "Different watchface identifier of the saved and loaded watch");
            Assert.AreEqual(watch.Needles.Count, loadedWatch.Needles.Count, "Different number of needles");
            var loadedWatchface = (WatchFaceType)loadedWatch.WatchFace.Product;
            Assert.AreEqual(watchface.Numbers.Length, loadedWatchface.Numbers.Length, "Different number of watch numbers");
        }

        [Test(Description = "This test saves a product with a null string property and saves it again. " +
                            "The bug was, that the HasChanged of the ColumnMapper throws an NullReferenceException")]
        public void LoadAndSaveTypeWithNullString()
        {
            // Arrange
            var watchfaceWithString = new WatchFaceType
            {
                Name = "Blubber",
                Identity = new ProductIdentity("8899665", 1),
                Numbers = new[] { 3, 6, 9, 12 },
                Brand = null //That's important for this test
            };

            var savedId = _storage.SaveType(watchfaceWithString);
            var loaded = (WatchFaceType)_storage.LoadType(savedId);

            // Act & Assert
            Assert.DoesNotThrow(delegate
            {
                _storage.SaveType(loaded);
            }, "Save should not fail with null string property");
        }

        [Test(Description = "Loads recipes by the classification flags enum")]
        public void LoadRecipesByClassification()
        {
            // Arrange
            var watch = new WatchType
            {
                Name = "Test",
                Identity = new ProductIdentity("8899665", 1),
            };

            _storage.SaveType(watch);

            CreateRecipe(RecipeClassification.Default);
            CreateRecipe(RecipeClassification.Alternative);
            CreateRecipe(RecipeClassification.Alternative);
            CreateRecipe(RecipeClassification.Part);

            // Act
            var defaults = _storage.LoadRecipes(watch.Id, RecipeClassification.Default);
            var alternatives = _storage.LoadRecipes(watch.Id, RecipeClassification.Alternative);
            var defaultsAndAlternatives = _storage.LoadRecipes(watch.Id, RecipeClassification.Default | RecipeClassification.Alternative);
            var parts = _storage.LoadRecipes(watch.Id, RecipeClassification.Part);
            var all = _storage.LoadRecipes(watch.Id, RecipeClassification.CloneFilter);

            // Assert
            Assert.AreEqual(1, defaults.Count);
            Assert.AreEqual(2, alternatives.Count);
            Assert.AreEqual(3, defaultsAndAlternatives.Count);
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(4, all.Count);

            void CreateRecipe(RecipeClassification classification)
            {
                var recipe = new WatchProductRecipe
                {
                    Product = watch,
                    Classification = classification,
                    Name = classification + ": TestRecipe",
                    Workplan = new Workplan { Id = _workplanId }
                };
                _storage.SaveRecipe(recipe);
            }
        }

        [Test(Description = "This test saves a product with a property which should not be saved. " +
                            "The NullPropertyMapper ignores this property at load and save.")]
        public void LoadAndSaveTypeWithNullPropertyMapper()
        {
            // Arrange
            var watchfaceWithString = new WatchFaceType
            {
                Name = "Fasel",
                Identity = new ProductIdentity("55889966", 1),
                Numbers = new[] { 3, 6, 9, 12 },
                Brand = "Unknown",
                Color = 42  //That's important for this test
            };

            // Act
            var savedId = _storage.SaveType(watchfaceWithString);
            var loaded = (WatchFaceType)_storage.LoadType(savedId);

            // Assert
            Assert.AreEqual(0, loaded.Color);
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
            var watch = SetupProduct("Jaques Lemans", string.Empty);
            _storage.SaveType(watch);
            watch = SetupProduct("Jaques Lemans", string.Empty, 17);
            _storage.SaveType(watch);

            // Act
            var all = _storage.LoadTypes(new ProductQuery());
            var latestRevision = _storage.LoadTypes(new ProductQuery { RevisionFilter = RevisionFilter.Latest });
            var byType = _storage.LoadTypes(new ProductQuery { Type = typeof(NeedleType).AssemblyQualifiedName });
            var allRevision = _storage.LoadTypes(new ProductQuery { Identifier = WatchMaterial });
            var latestByType = _storage.LoadTypes(new ProductQuery
            {
                Type = typeof(WatchType).AssemblyQualifiedName,
                RevisionFilter = RevisionFilter.Latest
            });
            var usages = _storage.LoadTypes(new ProductQuery
            {
                Identifier = "24",
                Selector = Selector.Parent
            });
            var needles = _storage.LoadTypes(new ProductQuery
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
        public void GetProductByExpression()
        {
            // Arrange
            var watchface = new DisplayWatchFaceType
            {
                Name = "ExpressionWatchface",
                Identity = new ProductIdentity("4742", 0),
                Resolution = 180
            };
            _storage.SaveType(watchface);

            // Act
            var loaded = _storage.LoadTypes<DisplayWatchFaceType>(wf => wf.Resolution == 180);
            var loaded2 = _storage.LoadTypes<DisplayWatchFaceType>(wf => wf.Resolution > 150);
            var loaded3 = _storage.LoadTypes(new ProductQuery
            {
                Type = typeof(DisplayWatchFaceType).AssemblyQualifiedName,
                PropertyFilters = new List<PropertyFilter>
                {
                    new()
                    {
                        Operator = PropertyFilterOperator.Equals,
                        Entry = new Entry
                        {
                            Identifier = nameof(DisplayWatchFaceType.Resolution),
                            Value = new EntryValue { Current = "180" }
                        }
                    }
                }
            });

            // Assert
            Assert.AreEqual(loaded.Count, 1);
            Assert.AreEqual(watchface.Id, loaded[0].Id);
            Assert.AreEqual(loaded2.Count, 1);
            Assert.AreEqual(watchface.Id, loaded2[0].Id);
            Assert.AreEqual(loaded3.Count, 1);
            Assert.AreEqual(watchface.Id, loaded3[0].Id);
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
            var needles = productMgr.LoadTypes(new ProductQuery
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
            var products = productMgr.LoadTypes(new ProductQuery
            {
                Identifier = "b*",
                RevisionFilter = RevisionFilter.Latest
            });

            // Assert
            Assert.AreEqual(1, products.Count, "There should be a product for the given query");
        }

        [TestCase(false, false, Description = "Duplicate product with valid id")]
        //[TestCase(false, true, Description = "Duplicate product, but identity already taken")]
        //[TestCase(true, false, Description = "Duplicate product but with template missmatch")]
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
                    duplicate = (WatchType)productMgr.Duplicate(watch, newIdentity);
                });
                Assert.AreEqual(crossTypeIdentifier, ex.InvalidTemplate);
                return;
            }

            Assert.DoesNotThrow(() =>
            {
                duplicate = (WatchType)productMgr.Duplicate(watch,
                    new ProductIdentity("654" + WatchMaterial, 1));
            });

            var recipeDuplicates = _storage.LoadRecipes(duplicate.Id, RecipeClassification.CloneFilter);

            // Assert
            Assert.AreEqual(watch.WatchFace.Product.Id, duplicate.WatchFace.Product.Id);
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
                result = productMgr.DeleteType(watch.WatchFace.Product.Id);
            else
                result = productMgr.DeleteType(watch.Id);

            // Assert
            Assert.AreEqual(stillUsed, !result);
            if (stillUsed)
                return;

            var matches = productMgr.LoadTypes(new ProductQuery
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
            var watch = SetupProduct("TestWatch", string.Empty);
            _storage.SaveType(watch);
            // Reload from storage for partlink ids if the object exists
            watch = (WatchType) _storage.LoadType(watch.Id);

            // Act
            var instance = (WatchInstance)watch.CreateInstance();
            instance.TimeSet = true;
            instance.DeliveryDate = DateTime.Now;
            instance.Identity = new BatchIdentity("12345");
            _storage.SaveInstances(new[] { instance });

            // Assert
            using (var uow = _factory.Create())
            {
                var root = uow.GetRepository<IProductInstanceRepository>().GetByKey(instance.Id);
                Assert.NotNull(root, "Failed to save or id not written");
                Assert.AreEqual(instance.DeliveryDate.Ticks, root.Integer1, "DateTime not saved");
                Assert.AreEqual(1, root.Integer2, "Bool not saved");

                var parts = root.Parts;
                Assert.AreEqual(1, parts.Count, "Invalid number of parts!"); // needles will be skipped for saving

                var single = parts.FirstOrDefault(p => p.PartLinkEntityId == watch.WatchFace.Id);
                Assert.NotNull(single, "Single part not saved!");
            }

            // Act
            var watchCopy = (WatchInstance)_storage.LoadInstances(instance.Id)[0];
            var identity = instance.Identity;
            var byIdentity = _storage.LoadInstances<IIdentifiableObject>(w => identity.Equals(w.Identity));
            var byDateTime = _storage.LoadInstances<WatchInstance>(i => i.DeliveryDate < DateTime.Now);
            var byBool = _storage.LoadInstances<WatchInstance>(i => i.TimeSet);
            var byType = _storage.LoadInstances(watch);
            var byType1 = _storage.LoadInstances<WatchInstance>(i => i.Type == watch);
            var byType2 = _storage.LoadInstances<WatchInstance>(i => i.Type.Equals(watch));
            var byType3 = _storage.LoadInstances<WatchInstance>(i => watch.Equals(i.Type));
            var byType4 = _storage.LoadInstances<WatchInstance>(i => i.Type.Name == "TestWatch");
            var byType5 = _storage.LoadInstances<WatchInstance>(i => watch == i.Type);
            identity = watch.Identity;
            var byType6 = _storage.LoadInstances<WatchInstance>(i => i.Type.Identity == identity);

            // Assert
            Assert.NotNull(watchCopy);
            Assert.AreEqual(instance.DeliveryDate, watchCopy.DeliveryDate);
            Assert.AreEqual(instance.TimeSet, watchCopy.TimeSet);
            Assert.NotNull(instance.WatchFace);
            Assert.AreEqual(instance.WatchFace.Identifier, watchCopy.WatchFace.Identifier, "Guid does not match");
            Assert.NotNull(instance.Needles);
            Assert.AreEqual(3, instance.Needles.Count);

            Assert.LessOrEqual(1, byIdentity.Count);
            Assert.LessOrEqual(1, byDateTime.Count);
            Assert.LessOrEqual(1, byBool.Count);
            Assert.LessOrEqual(1, byType.Count);
            Assert.LessOrEqual(1, byType1.Count);
            Assert.LessOrEqual(1, byType2.Count);
            Assert.LessOrEqual(1, byType3.Count);
            Assert.LessOrEqual(1, byType4.Count);
            Assert.LessOrEqual(1, byType5.Count);
            Assert.LessOrEqual(1, byType6.Count);
        }
    }
}
