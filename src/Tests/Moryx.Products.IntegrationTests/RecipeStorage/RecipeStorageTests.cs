// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Drawing;
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
            Assert.That(id1, Is.Not.EqualTo(id2));
            Assert.That(id1, Is.EqualTo(workplan.Id), "Id not assigned to original object!");
            using (var uow = _factory.Create())
            {
                var entity1 = uow.GetRepository<IWorkplanRepository>().GetByKey(id1);
                Assert.That(entity1.Name, Is.EqualTo(workplan.Name), "Name not correctly stored and saved");
            }
            Assert.That(id2, Is.EqualTo(loaded.Id), "Name not correctly stored and saved");
            Assert.That(loaded2.Name, Is.EqualTo(loaded.Name), "Name not correctly stored and saved");
            Assert.That(loaded.State, Is.EqualTo(workplan.State));
            Assert.That(loaded.MaxElementId, Is.EqualTo(workplan.MaxElementId));

            // Compare workplans
            var steps = workplan.Steps.ToList();
            var loadedSteps = loaded.Steps.ToList();
            foreach (var step in steps)
            {
                var loadedStep = loadedSteps.FirstOrDefault(s => s.Id == step.Id);
                Assert.That(loadedStep, Is.Not.Null);
                Assert.That(loadedStep.GetType(), Is.EqualTo(step.GetType()));
                Assert.That(loadedStep.Position.X, Is.EqualTo(step.Position.X), "Workplan step position: x coordinate not as expected");
                Assert.That(loadedStep.Position.Y, Is.EqualTo(step.Position.Y), "Workplan step position: y coordinate not as expected");

                for (int index = 0; index < step.Inputs.Length; index++)
                {
                    Assert.That(loadedStep.Inputs[index].Id, Is.EqualTo(step.Inputs[index].Id));
                    Assert.That(loadedStep.Inputs[index].Name, Is.EqualTo(step.Inputs[index].Name));
                }
                for (int index = 0; index < step.Outputs.Length; index++)
                {
                    Assert.That(loadedStep.Outputs[index].Id, Is.EqualTo(step.Outputs[index].Id));
                    Assert.That(loadedStep.Outputs[index].Name, Is.EqualTo(step.Outputs[index].Name));
                }
            }

            var startConnector = loaded.Connectors.FirstOrDefault(c => c.Name == "Start");
            Assert.That(startConnector.Position.X, Is.EqualTo(10), "Connector position: x coordingate not as expected");
            Assert.That(startConnector.Position.Y, Is.EqualTo(15), "Connector position: y coordingate not as expected");
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
            start.Position = new Point(10, 15);
            var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
            var inter1 = WorkplanInstance.CreateConnector("Inter1");
            var inter2 = WorkplanInstance.CreateConnector("Inter2");
            var inter3 = WorkplanInstance.CreateConnector("Inter3");
            workplan.Add(start, end, inter1, inter2, inter3);

            var mount = new TaskA
            {
                Inputs = { [0] = start },
                Outputs = { [0] = inter1, [1] = end, [2] = end },
                Position = new Point(1, 2),
            };

            var identity = new TaskB
            {
                Inputs = { [0] = inter1 },
                Outputs = { [0] = inter2, [1] = inter3, [2] = end },
                Position = new Point(3, 4),
            };

            var unmount1 = new TaskA
            {
                Inputs = {[0] = inter2},
                Outputs = {[0] = end, [1] = end, [2] = end },
                Position = new Point(5, 6),
            };

            var unmount2 = new TaskB
            {
                Inputs = { [0] = inter3 },
                Outputs = { [0] = end, [1] = end, [2] = end },
                Position = new Point(7, 8),
            };

            workplan.Add(mount, identity, unmount1, unmount2);

            return workplan;
        }
    }
}
