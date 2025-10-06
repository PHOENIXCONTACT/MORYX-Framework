// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Tools;
using Moryx.Workplans;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moryx.AbstractionLayer.Products.Endpoints.Tests
{
    [TestFixture]
    public class ProductConverterTests
    {
        private Mock<IProductManagement> _productManagerMock;

        private ProductConverter _productConverter;

        [SetUp]
        public void Setup()
        {
            _productManagerMock = new Mock<IProductManagement>();

            _productConverter = new ProductConverter(_productManagerMock.Object, null, null);
        }

        #region Products
        internal static IEnumerable<TestCaseData> ProductForwardBackwardConversionTestCaseGenerator()
        {
            // Used only to display tests seperately in TestExplorer
            var testCaseCounter = 0;

            // Create ProductType objects for test cases
            var dummyProductTypes = new List<DummyProductType>()
            {
                new DummyProductType(),
                new DummyProductTypeWithParts(),
                new DummyProductTypeWithParts()
                {
                    ProductPartLink = new DummyProductPartLink() { Id=1, Product = new DummyProductType() { Id = 2022 } },
                    ProductPartLinkEnumerable = new List<DummyProductPartLink>(){ new DummyProductPartLink() { Id=2, Product = new DummyProductType() { Id = 2023 } } }
                },
                new DummyProductTypeWithFiles(),
                new DummyProductTypeWithFiles() { FirstProductFile = new ProductFile() { Name="FirstFile" }, SecondProductFile = new ProductFile() { Name="SecondFile" }}
            };
            // Create Recipe objects for test cases
            var dummyRecipeLists = new List<List<IProductRecipe>>()
            {
                new List<IProductRecipe>(),
                new List<IProductRecipe>() { new ProductRecipe() { Id = 0 } },
                new List<IProductRecipe>() { new ProductRecipe() { Id = 1923 } }
            };

            // Create all possible combinations of input settings for the ConvertProduct method
            foreach (var identity in new IIdentity[] { null, new ProductIdentity("TestIdentifier", 1337) })
                foreach (ProductState state in Enum.GetValues(typeof(ProductState)))
                    foreach (var flat in new bool[] { true, false })
                        foreach (var dummyType in dummyProductTypes)
                            foreach (var recipes in dummyRecipeLists)
                                yield return new TestCaseData(dummyType, state, identity, recipes, flat, testCaseCounter++);
        }
        /// <summary>
        /// Test if the conversion to and back from a ProductModel without modification of the model 
        /// in between works without information loss.
        /// </summary>
        [TestCaseSource(nameof(ProductForwardBackwardConversionTestCaseGenerator))]
        public void ForwardBackwardProductConversionWithoutInformationLoss(DummyProductType originalProductType,
            ProductState state, IIdentity identity, IReadOnlyList<IProductRecipe> recipes, bool flat, int counter)
        {
            // Arrange
            // - Basic ProductType properties
            originalProductType.Id = 42;
            originalProductType.Name = "TestName";
            originalProductType.State = state;
            originalProductType.Identity = identity;
            // - Expected behavior from the RecipeManagement
            if (recipes.Any())
                ReflectionTool.TestMode = true;
            _productManagerMock.Setup(rm => rm.GetRecipes(It.IsAny<IProductType>(), RecipeClassification.CloneFilter)).Returns(recipes);
            _productManagerMock.Setup(rm => rm.LoadRecipe(It.IsAny<long>())).Returns((long id) => new DummyProductRecipe() { Id = id });
            // - Create target ProductType object
            var targetDummyProductType = (DummyProductType)Activator.CreateInstance(originalProductType.GetType());
            targetDummyProductType.Id = 42;
            // - Expected behavior from the RecipeManagement
            _productManagerMock.Setup(pm => pm.LoadType(It.IsAny<long>())).Returns((long id) => new DummyProductType() { Id = id });
            // - Product PartsShould only by included with their id if they already exist
            var originalProductTypeWithParts = originalProductType as DummyProductTypeWithParts;
            if (originalProductTypeWithParts is not null && originalProductTypeWithParts.ProductPartLink is not null)
            {
                ((DummyProductTypeWithParts)targetDummyProductType).ProductPartLink = ((DummyProductTypeWithParts)originalProductType).ProductPartLink;
                ((DummyProductTypeWithParts)targetDummyProductType).ProductPartLinkEnumerable = ((DummyProductTypeWithParts)originalProductType).ProductPartLinkEnumerable.Take(1).ToList();
            }

            // Act
            var convertedModel = _productConverter.ConvertProduct(originalProductType, flat);
            var recoveredOriginal = _productConverter.ConvertProductBack(convertedModel, targetDummyProductType);

            // Assert
            // - Non ProductIdentities will always be transformed into empty ProductIdentities in the Process
            if (identity is null)
                originalProductType.Identity = new ProductIdentity("", 0);
            // - Products originally converted with the flat flag will only affect certain properties
            if (flat)
            {
                Assert.That(recoveredOriginal.Id, Is.EqualTo(originalProductType.Id));
                Assert.That(recoveredOriginal.Name, Is.EqualTo(originalProductType.Name));
                Assert.That(recoveredOriginal.State, Is.EqualTo(originalProductType.State));
                Assert.That(recoveredOriginal.Identity, Is.EqualTo(originalProductType.Identity));
                _productManagerMock.VerifyNoOtherCalls();
                _productManagerMock.VerifyNoOtherCalls();
                return;
            }
            // - The following assert uses overwritten Equals methods to check for VALUE equality
            Assert.That(recoveredOriginal, Is.EqualTo(originalProductType));
            // - If there are Recipes the RecipeManagement should be called
            if (recipes.Any())
            {
                _productManagerMock.Verify(rm => rm.GetRecipes(originalProductType, RecipeClassification.CloneFilter));
                _productManagerMock.Verify(rm => rm.SaveRecipe(It.Is<IProductRecipe>(recipe => !HasChangedProperties<IProductRecipe>(recipe, recipes.LastOrDefault()))));
                if (recipes.First().Id != 0)
                    _productManagerMock.Verify(rm => rm.LoadRecipe(recipes.First().Id));
            }
            // - If there are ProductPartLinks the ProductManagement should be called
            var targetDummyTypeWithParts = recoveredOriginal as DummyProductTypeWithParts;
            if (targetDummyTypeWithParts?.ProductPartLink?.Product is not null)
                _productManagerMock.Verify(pm => pm.LoadType(targetDummyTypeWithParts.ProductPartLink.Product.Id));
        }

        private static bool HasChangedProperties<T>(object A, object B)
        {
            if (A is null || B is null)
                throw new ArgumentNullException("You need to provide 2 non-null objects");

            var type = typeof(T);
            var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var allSimpleProperties = allProperties.Where(pi => IsSimpleType(pi.PropertyType));
            var unequalProperties =
                    from pi in allSimpleProperties
                    let AValue = type.GetProperty(pi.Name).GetValue(A, null)
                    let BValue = type.GetProperty(pi.Name).GetValue(B, null)
                    where AValue != BValue && (AValue == null || !AValue.Equals(BValue))
                    select pi.Name;
            return unequalProperties.Any();
        }

        private static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                // nullable type, check if the nested type is simple.
                return IsSimpleType(type.GetGenericArguments()[0]);

            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }


        #endregion

        #region Recipes
        public static IEnumerable<TestCaseData> RecipeForwardBackwardConversionTestCaseGenerator()
        {
            // Used only to display tests seperately in TestExplorer
            var testCaseCounter = 0;

            // Create Recipe objects for test cases
            var dummyRecipes = new List<DummyProductRecipe>()
            {
                new DummyProductRecipe(),
                new DummyProductWorkplanRecipe() { Workplan = new DummyWorkplan() { Id=2021 } }
            };
            // Create all classifications to consider
            var classifications = Enum.GetValues(typeof(RecipeClassification)).Cast<RecipeClassification>()
                .Where(c => c != RecipeClassification.Clone && c != RecipeClassification.CloneFilter).ToList();
            var clonedClassifications = classifications.Select(c => c | RecipeClassification.Clone).ToList();
            classifications.AddRange(clonedClassifications);

            // Create all possible combinations of input settings for the ConvertRecipe method
            foreach (var backupProductType in new List<DummyProductType>() { null, new DummyProductType() })
                foreach (RecipeState state in Enum.GetValues(typeof(RecipeState)))
                    foreach (var classification in classifications)
                        foreach (var dummyRecipe in dummyRecipes)
                        {
                            //dummyRecipe.Classification = classification;
                            //dummyRecipe.State = state;
                            var workplanInTargetRecipe = (dummyRecipe as DummyProductWorkplanRecipe)?.Workplan;
                            if (workplanInTargetRecipe is not null)
                                yield return new TestCaseData(dummyRecipe, classification, state, backupProductType, new DummyWorkplan() { Id = 1 }, testCaseCounter++);
                            yield return new TestCaseData(dummyRecipe, classification, state, backupProductType, workplanInTargetRecipe, testCaseCounter++);
                        }
        }

        //Problem Entry Convert
        [TestCaseSource(nameof(RecipeForwardBackwardConversionTestCaseGenerator))]
        public void ForwardBackwardRecipeConversionWithoutInformationLoss(DummyProductRecipe originalRecipe, RecipeClassification classification, RecipeState state,
            DummyProductType backupProductType, DummyWorkplan workplanInTargetRecipe, int testCaseCounter)
        {
            // Arrange
            // - Basic Workplan properties
            originalRecipe.Id = 42;
            originalRecipe.Name = "TestName";
            originalRecipe.Revision = 1337;
            originalRecipe.Classification = classification;
            originalRecipe.State = state;
            // - Mock workplan requests if there could be a Workplan
            var originalWorkplanRecipe = originalRecipe as DummyProductWorkplanRecipe;
            if (originalWorkplanRecipe is not null)
            {
                _productManagerMock.Setup(pm => pm.LoadWorkplan(It.IsAny<long>()))
                    .Returns((long id) => new DummyWorkplan() { Id = id });
            }
            // - Create target object
            var targetDummyRecipe = (DummyProductRecipe)Activator.CreateInstance(originalRecipe.GetType());
            targetDummyRecipe.Id = 42;
            if (originalWorkplanRecipe is not null)
                ((DummyProductWorkplanRecipe)targetDummyRecipe).Workplan = workplanInTargetRecipe;
            // - No change of classification on clones should be possible, thereby preset it 
            if (originalRecipe.Classification.HasFlag(RecipeClassification.Clone))
                targetDummyRecipe.Classification = originalRecipe.Classification;


            // Act
            var convertedModel = _productConverter.ConvertRecipeV2(originalRecipe);
            var recoveredOriginal = _productConverter.ConvertRecipeBack(convertedModel, targetDummyRecipe, backupProductType);


            // Assert
            // - Backup products are used for recipes without products
            if (originalRecipe.Product is null)
                originalRecipe.Product = backupProductType;

            Assert.That(recoveredOriginal, Is.EqualTo(originalRecipe));
            // - If there is a workplan and it changed, reload it at backward conversion
            if (originalWorkplanRecipe?.Workplan is not null && originalWorkplanRecipe.Workplan.Id != workplanInTargetRecipe.Id)
                _productManagerMock.Verify(wm => wm.LoadWorkplan(originalWorkplanRecipe.Workplan.Id), Times.Once);
            else
                _productManagerMock.VerifyNoOtherCalls();
        }
        #endregion

        #region Workplans
        public static IEnumerable<TestCaseData> WorkplanForwardConversionTestCaseGenerator()
        {
            foreach (var wpState in Enum.GetValues(typeof(WorkplanState)))
                yield return new TestCaseData(wpState);
        }

        //Problem Entry Convert
        [TestCaseSource(nameof(WorkplanForwardConversionTestCaseGenerator))]
        public void ForwardWorkplanConversionWithoutInformationLoss(WorkplanState wpState)
        {
            // Arrange
            var originalWorkplan = new DummyWorkplan()
            {
                Id = 42,
                Name = "TestWorkplan",
                Version = 1,
                State = wpState
            };

            // Act
            var convertedmodel = ProductConverter.ConvertWorkplan(originalWorkplan);

            // Assert
            Assert.That(convertedmodel.Id, Is.EqualTo(originalWorkplan.Id));
            Assert.That(convertedmodel.Name, Is.EqualTo(originalWorkplan.Name));
            Assert.That(convertedmodel.Version, Is.EqualTo(originalWorkplan.Version));
            Assert.That(convertedmodel.State, Is.EqualTo(originalWorkplan.State));
        }
        #endregion
    }
}