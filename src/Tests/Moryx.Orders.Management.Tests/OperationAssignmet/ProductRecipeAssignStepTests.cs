// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.TestTools;
using Moq;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;
using Moryx.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class ProductRecipeAssignStepTests
    {
        private ProductAssignStep _productAssignStep;
        private RecipeAssignStep _recipeAssignStep;

        private Mock<IProductManagement> _productManagementMock;
        private Mock<IJobManagement> _jobManagementMock;
        private Mock<IOperationData> _operationDataMock;
        private ProductIdentity _productIdentity;
        private ProductReference _productReference;
        private IOperationData _operationData;
        private IOperationLogger _operationLogger;
        private Mock<IOperationDataPool> _operationPoolMock;
        private InternalOperation _operation;

        [SetUp]
        public void SetUp()
        {
            var operationLoggerMock = new Mock<IOperationLogger>();
            _operationLogger = operationLoggerMock.Object;

            // Prepare test data for each test
            _productIdentity = new ProductIdentity("TestProduct", 1);
            _productReference = new ProductReference(_productIdentity);

            _operation = new InternalOperation
            {
                Product = _productReference
            };

            _operationDataMock = new Mock<IOperationData>();
            _operationDataMock.SetupGet(o => o.Operation).Returns(_operation);
            _operationDataMock.SetupGet(o => o.Product).Returns(_productReference);
            _operationData = _operationDataMock.Object;

            _jobManagementMock = new Mock<IJobManagement>();
            _jobManagementMock.Setup(j => j.Evaluate(It.IsAny<IProductionRecipe>(), It.IsAny<int>()))
                .Returns(new JobEvaluation
                {
                    WorkplanErrors = new List<string>()
                });

            _operationPoolMock = new Mock<IOperationDataPool>();

            _productManagementMock = new Mock<IProductManagement>();
            var productAssignment = new DefaultProductAssignment
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                ProductManagement = _productManagementMock.Object
            };
            productAssignment.Start();

            _productAssignStep = new ProductAssignStep
            {
                ProductAssignment = productAssignment,
                ProductManagement = _productManagementMock.Object
            };

            _productAssignStep.Start();

            var recipeAssignment = new DefaultRecipeAssignment
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                ProductManagement = _productManagementMock.Object
            };
            recipeAssignment.Start();

            _recipeAssignStep = new RecipeAssignStep
            {
                RecipeAssignment = recipeAssignment,
                ProductManagement = _productManagementMock.Object,
                JobManagement = _jobManagementMock.Object,
                OperationDataPool = _operationPoolMock.Object
            };
            _recipeAssignStep.Start();
        }

        [Test(Description = "Should assign a product by a given reference with the identifier and assign the default recipe")]
        public async Task AssignProductAndRecipe()
        {
            // Arrange
            _productManagementMock.Setup(m => m.LoadType(It.IsAny<ProductIdentity>())).Returns(new DummyProductType());
            _productManagementMock.Setup(m => m.GetRecipes(It.IsAny<ProductType>(), RecipeClassification.Default)).Returns([new DummyRecipe()]);

            // Act
            var productAssigned = await _productAssignStep.AssignStep(_operationData, _operationLogger);
            var recipeAssigned = await _recipeAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(productAssigned, "The product assignment should be successful");
            Assert.That(recipeAssigned, "The recipe assignment should be successful");
        }

        [Test(Description = "Assigns a preselected recipe from the creation context information")]
        public async Task AssignPreselectedRecipe()
        {
            // Arrange
            const long preselection = 42;

            _operation.CreationContext = new OperationCreationContext { RecipePreselection = preselection };
            _operation.Recipes = new List<IProductRecipe>();

            _productManagementMock.Setup(m => m.LoadRecipe(preselection)).Returns(() => new DummyRecipe());

            // Act
            var recipeAssigned = await _recipeAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(recipeAssigned, "The recipe assignment should be successful");
            _productManagementMock.Verify(m => m.LoadRecipe(preselection), Times.Once, "Preselection was ignored by assignment");
        }

        [Test(Description = "Assigns a preselected recipe from the existing recipe collection of the operation")]
        public async Task AssignPreselectedRecipeOnReassign()
        {
            // Arrange
            const long preselection = 42;
            var template = new DummyRecipe
            {
                TemplateId = preselection
            };

            _operation.Recipes = new List<IProductRecipe> { template };
            _productManagementMock.Setup(m => m.LoadRecipe(preselection)).Returns(() => new DummyRecipe());

            // Act
            var recipeAssigned = await _recipeAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(recipeAssigned, "The recipe assignment should be successful");
            _productManagementMock.Verify(m => m.LoadRecipe(preselection), Times.Once, "Preselection was ignored by assignment");
        }

        [Test(Description = "Should restore a product by its id and and select a recipe which is the current recipe of the product")]
        public async Task RestoreProductAndRecipe()
        {
            // Arrange
            _productReference.Id = 1;
            _productManagementMock.Setup(m => m.LoadType(1)).Returns(new DummyProductType());
            _productManagementMock.Setup(m => m.LoadRecipe(1)).Returns(new DummyRecipe());

            _operation.Recipes = new List<IProductRecipe> { new ProductRecipeReference(1) };

            ProductType product;
            _operationDataMock.Setup(o => o.AssignProduct(It.IsAny<ProductType>())).Callback((ProductType productType) =>
            {
                product = productType;
                _operationDataMock.SetupGet(o => o.Product).Returns(product);
            });

            // Act
            var productAssigned = await _productAssignStep.RestoreStep(_operationData, _operationLogger);
            var recipeAssigned = await _recipeAssignStep.RestoreStep(_operationData, _operationLogger);

            // Assert
            Assert.That(productAssigned, "The product assignment should be successful");
            Assert.That(recipeAssigned, "The recipe assignment should be successful");
            Assert.That(_operationData.Product, Is.InstanceOf<DummyProductType>(), "There should be an assigned DummyProduct");
            Assert.That(_operation.Recipes.Single(), Is.InstanceOf<DummyRecipe>(), "There should be a selected DummyRecipe");
        }

        [Test(Description = "Should fail to assign an unknown product")]
        public async Task FailAssignmentByAnUnknownProduct()
        {
            // Arrange
            _productManagementMock.Setup(m => m.LoadType(_productIdentity)).Returns((ProductType)null);
            _productManagementMock.Setup(m => m.GetRecipes(It.IsAny<ProductType>(), RecipeClassification.Default)).Returns(Array.Empty<IProductRecipe>());

            _operation.Recipes = new List<IProductRecipe>(1) { new ProductRecipeReference(0) };

            // Act
            var productAssigned = await _productAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(false, Is.EqualTo(productAssigned), "The product assignment should be failed");
            Assert.That(_operationData.Product, Is.InstanceOf<ProductReference>(), "There should be still a ProductTypeReference after a failed assignment");
        }

        [Test(Description = "Should fail to assign a product with an unknown identity")]
        public async Task FailAssignmentByAnUnknownProductIdentity()
        {
            // Arrange
            _operationDataMock.SetupGet(o => o.Product).Returns(new ProductReference(null));

            // Act
            var productAssigned = await _productAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(productAssigned, Is.False, "The product assignment should be failed");
            Assert.That(_operationData.Product, Is.InstanceOf<ProductReference>(), "There should be still a ProductReference after a failed assignment");
        }

        [Test(Description = "Should fail to assign an unknown recipe")]
        public async Task FailAssignmentByAnUnknownRecipe()
        {
            // Arrange
            _productManagementMock.Setup(m => m.LoadType(_productIdentity)).Returns(new DummyProductType());
            _productManagementMock.Setup(m => m.GetRecipes(It.IsAny<ProductType>(), RecipeClassification.Default)).Returns(Array.Empty<IProductRecipe>());

            _operation.Recipes = new List<IProductRecipe>(1) { new ProductRecipeReference(0) };
            await _productAssignStep.AssignStep(_operationData, _operationLogger);

            // Act
            var recipeAssigned = await _recipeAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(recipeAssigned, Is.False, "The recipe assignment should be failed");
            Assert.That(_operation.Recipes.Single(), Is.InstanceOf<RecipeReference>(), "There should be still RecipeReference after a failed assignment");
        }

        [Test(Description = "Recipe should always be cloned while assigning an recipe")]
        public async Task RecipeShouldBeClonedAfterSelection()
        {
            // Arrange
            _productManagementMock.Setup(m => m.LoadType(_productIdentity)).Returns(new DummyProductType());
            _productManagementMock.Setup(m => m.GetRecipes(It.IsAny<ProductType>(), RecipeClassification.Default)).Returns([new DummyRecipe()]);

            IProductRecipe clonedRecipe = null;
            _productManagementMock.Setup(m => m.SaveRecipe(It.IsAny<IProductRecipe>())).Callback(
                delegate (IProductRecipe recipe) { clonedRecipe = recipe; });

            _operationDataMock.Setup(o => o.AssignRecipes(It.IsAny<IReadOnlyList<IProductRecipe>>())).Callback((IReadOnlyList<IProductRecipe> newRecipes) =>
            {
                _operation.Recipes = newRecipes.ToArray();
            });

            // Act
            await _recipeAssignStep.AssignStep(_operationData, _operationLogger);

            // Assert
            Assert.That(clonedRecipe, Is.Not.Null, "Recipe clone was not created.");

            var assignedRecipe = _operation.Recipes.Single();
            Assert.That(clonedRecipe, Is.EqualTo(assignedRecipe), "Clone does not match the assigned recipe.");
            Assert.That(assignedRecipe.Classification.HasFlag(RecipeClassification.Clone), "Recipe was not cloned");
        }

        [TestCase(0, Description = "Recipe change should lead to an operation depending event from the recipe assignment")]
        [TestCase(1, Description = "Recipe clone change should lead to an operation depending event from the recipe assignment")]
        public void ShouldInvokeRecipeChange(long templateId)
        {
            // Arrange
            var recipe = DummyRecipe.BuildRecipe();
            recipe.TemplateId = templateId;
            recipe.Id = templateId + 1;
            var clone = DummyRecipe.BuildRecipe();
            clone.TemplateId = 1;
            clone.Id = 2;

            var completedOperation = new InternalOperation();
            var completedOperationDataMock = new Mock<IOperationData>();
            completedOperationDataMock.SetupGet(o => o.Operation).Returns(completedOperation);
            completedOperationDataMock.SetupGet(o => o.State.Classification).Returns(OperationClassification.Completed);

            var runningOperation = new InternalOperation();
            var runningOperationDataMock = new Mock<IOperationData>();
            runningOperationDataMock.SetupGet(o => o.Operation).Returns(runningOperation);
            runningOperationDataMock.SetupGet(o => o.State.Classification).Returns(OperationClassification.Running);

            runningOperation.Recipes = new List<IProductRecipe> { clone };

            var operations = new List<IOperationData>
            {
                completedOperationDataMock.Object,
                runningOperationDataMock.Object
            };

            _operationPoolMock.Setup(o => o.GetAll(It.IsAny<Func<IOperationData, bool>>())).Returns(
                (Func<IOperationData, bool> filter) => operations.Where(filter).ToArray());

            // Act
            _productManagementMock.Raise(p => p.RecipeChanged += null, _productManagementMock.Object, recipe);

            // Assert
            runningOperationDataMock.Verify(o => o.RecipeChanged(recipe), Times.Once, "There should be an operation depending recipe change");
        }
    }
}

