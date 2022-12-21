// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Products.Management;
using Moryx.Products.Model;
using Moryx.Workplans;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests
{
    [TestFixture]
    public class RecipeStorageTests
    {
        private IUnitOfWorkFactory<ProductsContext> _factory;

        private RecipeManagement _recipeManagement;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            _factory = BuildUnitOfWorkFactory();

            var storageMock = new Mock<IProductStorage>();
            storageMock.Setup(sm => sm.LoadRecipe(It.IsAny<long>())).Returns(new ProductionRecipe());

            _recipeManagement = new RecipeManagement()
            {
                ModelFactory = _factory,
                Storage = storageMock.Object
            };
        }

        protected virtual UnitOfWorkFactory<ProductsContext> BuildUnitOfWorkFactory()
        {
            return new UnitOfWorkFactory<ProductsContext>(new InMemoryDbContextManager("RecipeStorageTests"));
        }

        [Test]
        public void FullCycle()
        {
            // Arrange
            var workplan = CreateWorkplan();

            // Act
            Workplan loaded, loaded2;
            long id1, id2;
            id1 = _recipeManagement.SaveWorkplan(workplan);
            loaded = _recipeManagement.LoadWorkplan(id1);

            loaded.Name = "Modified";

            id2 = _recipeManagement.SaveWorkplan(loaded);
            loaded2 = _recipeManagement.LoadWorkplan(id2);

            // Assert
            Assert.AreNotEqual(id1, id2);
            Assert.AreEqual(workplan.Id, id1, "Id not assigned to original object!");
            using (var uow = _factory.Create())
            {
                var entity1 = uow.GetRepository<IWorkplanRepository>().GetByKey(id1);
                Assert.AreEqual(workplan.Name, entity1.Name, "Name not correctly stored and saved");
            }
            Assert.AreEqual(loaded.Id, id2, "Name not correctly stored and saved");
            Assert.AreEqual(loaded.Name, loaded2.Name, "Name not correctly stored and saved");
            Assert.AreEqual(workplan.State, loaded.State);
            Assert.AreEqual(workplan.MaxElementId, loaded.MaxElementId);

            // Compare workplans
            var steps = workplan.Steps.ToList();
            var loadedSteps = loaded.Steps.ToList();
            foreach (var step in steps)
            {
                var loadedStep = loadedSteps.FirstOrDefault(s => s.Id == step.Id);
                Assert.NotNull(loadedStep);
                Assert.AreEqual(step.GetType(), loadedStep.GetType());
                for (int index = 0; index < step.Inputs.Length; index++)
                {
                    Assert.AreEqual(step.Inputs[index].Id, loadedStep.Inputs[index].Id);
                    Assert.AreEqual(step.Inputs[index].Name, loadedStep.Inputs[index].Name);
                }
                for (int index = 0; index < step.Outputs.Length; index++)
                {
                    Assert.AreEqual(step.Outputs[index].Id, loadedStep.Outputs[index].Id);
                    Assert.AreEqual(step.Outputs[index].Name, loadedStep.Outputs[index].Name);
                }
            }
        }

        private static Workplan CreateWorkplan()
        {
            var workplan = new Workplan
            {
                Name = "Dummy",
                Version = 42,
                State = WorkplanState.Released
            };

            var start = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
            var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
            var inter1 = WorkplanInstance.CreateConnector("Inter1");
            var inter2 = WorkplanInstance.CreateConnector("Inter2");
            var inter3 = WorkplanInstance.CreateConnector("Inter3");
            workplan.Add(start, end, inter1, inter2, inter3);

            var mount = new TaskA
            {
                Inputs = { [0] = start },
                Outputs = { [0] = inter1, [1] = end, [2] = end }
            };

            var identity = new TaskB
            {
                Inputs = { [0] = inter1 },
                Outputs = { [0] = inter2, [1] = inter3, [2] = end }
            };

            var unmount1 = new TaskA
            {
                Inputs = {[0] = inter2},
                Outputs = {[0] = end, [1] = end, [2] = end }
            };

            var unmount2 = new TaskB
            {
                Inputs = { [0] = inter3 },
                Outputs = { [0] = end, [1] = end, [2] = end }
            };

            workplan.Add(mount, identity, unmount1, unmount2);

            return workplan;
        }
    }
}
