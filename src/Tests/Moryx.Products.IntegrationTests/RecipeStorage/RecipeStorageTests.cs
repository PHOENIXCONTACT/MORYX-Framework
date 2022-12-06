// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
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

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            _factory = BuildUnitOfWorkFactory();
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
            WorkplanEntity entity1, entity2;
            using (var uow = _factory.Create())
            {
                entity1 = RecipeStorage.SaveWorkplan(uow, workplan);
                uow.SaveChanges();
                loaded = RecipeStorage.LoadWorkplan(uow, entity1.Id);

                loaded.Name = "Modified";

                entity2 = RecipeStorage.SaveWorkplan(uow, loaded);
                uow.SaveChanges();
                loaded2 = RecipeStorage.LoadWorkplan(uow, entity2.Id);
            }

            // Assert
            Assert.AreNotEqual(entity1.Id, entity2.Id);
            // TODO: This test seems a bit off, since the workplan object is not DB-related
            //Assert.AreEqual(workplan.Id, entity1.Id, "Id not assigned to original object!");
            Assert.AreEqual(workplan.Name, entity1.Name, "Name not correctly stored and saved");
            Assert.AreEqual(loaded.Name, entity2.Name, "Name not correctly stored and saved");
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

            var start = Workflow.CreateConnector("Start", NodeClassification.Start);
            var end = Workflow.CreateConnector("End", NodeClassification.End);
            var inter1 = Workflow.CreateConnector("Inter1");
            var inter2 = Workflow.CreateConnector("Inter2");
            var inter3 = Workflow.CreateConnector("Inter3");
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
