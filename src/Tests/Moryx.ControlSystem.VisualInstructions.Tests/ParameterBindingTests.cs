// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.ControlSystem.TestTools.Identity;
using Moryx.ControlSystem.VisualInstructions;
using NUnit.Framework;

namespace Moryx.Resources.AssemblyInstruction.Tests
{
    [TestFixture]
    public class ParameterBindingTests
    {
        [Test]
        public void ResolveIds()
        {
            // Arrange
            var process = DummyProcess();
            var mountingParameters = new MountingParameters()
            {
                Instructions =
                [
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = "Process with Id: {Process.Id} and recipe id: {Recipe.Id}!"
                    },
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = "Article id {Article.Id} of product {Product.Id}"
                    }
                ]
            };

            // Act
            var resolved = mountingParameters.Bind(process);

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<MountingParameters>());
            var resolvedMounting = (MountingParameters)resolved;
            Assert.That(resolvedMounting.Instructions.Length, Is.EqualTo(2));
            Assert.That(resolvedMounting.Instructions[0].Content, Is.EqualTo("Process with Id: 42 and recipe id: 42!"));
            Assert.That(resolvedMounting.Instructions[1].Content, Is.EqualTo("Article id 42 of product 42"));
        }

        [Test]
        public void ResolveSpecific()
        {
            // Arrange
            var process = DummyProcess();
            var mountingParameters = new MountingParameters()
            {
                Instructions =
                [
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = "Process of recipe {Recipe.Name} performed operations with Id: {ProductInstance.Id}!"
                    },
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = "ProductInstance with serial {ProductInstance.Identity} of product {Product.Name} in state: {ProductInstance.State}"
                    }
                ]
            };

            // Act
            var resolved = mountingParameters.Bind(process);

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<MountingParameters>());
            var resolvedMounting = (MountingParameters)resolved;
            Assert.That(resolvedMounting.Instructions.Length, Is.EqualTo(2));
            Assert.That(resolvedMounting.Instructions[0].Content, Is.EqualTo("Process of recipe AwesomeRecipe performed operations with Id: 42!"));
            Assert.That(resolvedMounting.Instructions[1].Content, Is.EqualTo("ProductInstance with serial 123456 of product Bob in state: Success"));
        }

        [Test]
        public void ResolveInImage()
        {
            // Arrange
            var process = DummyProcess();
            var mountingParameters = new MountingParameters()
            {
                Instructions =
                [
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Media,
                        Content = "C:\\Images\\{Product.Identifier}.png"
                    }
                ]
            };

            // Act
            var resolved = mountingParameters.Bind(process);

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<MountingParameters>());
            var resolvedMounting = (MountingParameters)resolved;
            Assert.That(resolvedMounting.Instructions.Length, Is.EqualTo(1));
            Assert.That(resolvedMounting.Instructions[0].Content, Is.EqualTo("C:\\Images\\123456.png"));
        }

        [Test]
        public void ResolveCustom()
        {
            // Arrange
            var process = DummyProcess();
            var mountingParameters = new MountingParameters()
            {
                Instructions =
                [
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = "Article blah is {Article.Blah} and a product Foo of {Product.Foo} for OrderNumber {Recipe.OrderNumber}!"
                    }
                ]
            };

            // Act
            var resolved = mountingParameters.Bind(process);

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<MountingParameters>());
            var resolvedMounting = (MountingParameters)resolved;
            Assert.That(resolvedMounting.Instructions.Length, Is.EqualTo(1));
            var blahValue = ((DummyProductInstance)((ProductionProcess)process).ProductInstance).Blah;
            Assert.That(resolvedMounting.Instructions[0].Content, Is.EqualTo($"Article blah is {blahValue} and a product Foo of 1337 for OrderNumber 123321!"));
        }

        [TestCase("Article {Article.Identity} completed in {Resource.Name}!", "Article 123456 completed in {Resource.Name}!", Description = "Preserve resource key!")]
        [TestCase("ProductInstance {Process.ProductInstance.Identity}", "ProductInstance 123456", Description = "Deep binding")]
        [TestCase("Null: {Product.Foo.Identity}", "Null: {Product.Foo.Identity}", Description = "Null somewhere")]
        [TestCase("Index: '{Product.Bobs[3]}'", "Index: '42'", Description = "Use numeric Indexer")]
        [TestCase("Index: '{Recipe.Config[Foo]}'", "Index: 'Blah'", Description = "Use text Indexer")]
        [TestCase("Format: '{Product.Id:D4}'", "Format: '0042'", Description = "Use text formatting")]
        public void VariousTests(string input, string output)
        {
            // Arrange
            var process = DummyProcess();
            var mountingParameters = new MountingParameters()
            {
                Instructions =
                [
                    new VisualInstruction
                    {
                        Type = InstructionContentType.Text,
                        Content = input
                    }
                ]
            };

            // Act
            var resolved = mountingParameters.Bind(process);

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<MountingParameters>());
            var resolvedMounting = (MountingParameters)resolved;
            Assert.That(resolvedMounting.Instructions.Length, Is.EqualTo(1));
            Assert.That(resolvedMounting.Instructions[0].Content, Is.EqualTo(output));
        }

        [TestCase("ProductInstance.Type", typeof(DummyProduct))]
        [TestCase("Recipe.Config[Foo].Length", typeof(int))]
        [TestCase("Product.Part.Product.Name", typeof(string))]
        [TestCase("Product.Part.Name", typeof(string), Description = "Use part link shortcut")]
        [TestCase("Process.LastActivity", typeof(AssignIdentityActivity))]
        [TestCase("Process.LastActivity.Tracing", typeof(Tracing))]
        [TestCase("Process.LastActivity[MountActivity]", typeof(MountActivity))]
        public void PlainResolve(string path, Type resultType)
        {
            // Arrange
            var resolver = new ProcessBindingResolverFactory().Create(path);

            // Act
            var process = DummyProcess();
            var result = resolver.Resolve(process);

            // Assert
            Assert.That(result.GetType(), Is.EqualTo(resultType), "Wrong object resolved");
        }

        [Test(Description = "Test for identifier, order of resolver is important here")]
        public void WrongOrderOfResolver()
        {
            // Arrange
            var resolver = new ProcessBindingResolverFactory().Create("Product.Part.Identifier");

            // Assert
            Assert.DoesNotThrow(() => resolver.Resolve(DummyProcess()));
        }

        [Test]
        public void NameConflict()
        {
            // Arrange
            var resolver = new ProcessBindingResolverFactory().Create("Product.Part.Id");

            // Assert
            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(DummyProcess()));
        }

        [Test(Description = "Remove Product shortcut resolve if source is neither Article nor Process")]
        public void ShortcutRemoval()
        {
            // Arrange
            var resolver = new ProcessBindingResolverFactory().Create("Article.Type.Dummy.Product.Name");
            var process = DummyProcess();
            int oldCount = 0, newCount = 0;

            // Act
            var current = (IBindingResolverChain)resolver;
            while (current.NextResolver != null)
            {
                oldCount++;
                current = current.NextResolver;
            }
            var result = resolver.Resolve(process);
            current = (IBindingResolverChain)resolver;
            while (current.NextResolver != null)
            {
                newCount++;
                current = current.NextResolver;
            }

            // Assert
            Assert.That(result, Is.EqualTo("Bob"));
            Assert.That(newCount, Is.LessThanOrEqualTo(oldCount));
            Assert.That(oldCount, Is.EqualTo(8));
            Assert.That(newCount, Is.EqualTo(4));
        }

        public class InsertPartParameters : Parameters
        {
            private IBindingResolver _resolver;

            public string Part { get; set; }  // e.g. Product.Part.Product

            public IProductType Target { get; set; }

            protected override void Populate(IProcess process, Parameters instance)
            {
                if (_resolver == null)
                    _resolver = ResolverFactory.Create(Part);

                var parameters = (InsertPartParameters) instance;
                parameters.Target = (IProductType) _resolver.Resolve(process);
            }
        }

        private static IProcess DummyProcess()
        {
            var product = new DummyProduct()
            {
                Id = 42,
                Name = "Bob",
                Foo = 1337,
                Bobs = [1, 3, 7, 42, 1337],
                Identity = new ProductIdentity("123456", 01),
                Part = new ProductPartLink<DummyProduct>(2)
                {
                    Product = new DummyProduct
                    {
                        Identity = new ProductIdentity("654321", 01),
                        Name = "Thomas",
                        Foo = 42
                    }
                }
            };
            var process = new ProductionProcess
            {
                Id = 42,
                Recipe = new DummyRecipe
                {
                    Id = 42,
                    Name = "AwesomeRecipe",
                    OrderNumber = "123321",
                    Product = product,
                    Config = new Dictionary<string, string> { { "Foo", "Blah" } }
                },
                ProductInstance = product.CreateInstance()
            };

            var instance = ((DummyProductInstance) process.ProductInstance);

            instance.Id = 42;
            instance.State = ProductInstanceState.Success;
            instance.Identity = new TestNumberIdentity(0, "123456");
            instance.Blah = (float)0.815;

            process.AddActivity(new MountActivity() { Id = 42 });
            process.AddActivity(new AssignIdentityActivity() { Result = ActivityResult.Create(AssignIdentityResults.Assigned) });
            process.AddActivity(new UnmountActivity());

            return process;
        }

        private class DummyRecipe : ProductionRecipe
        {
            public string OrderNumber { get; set; }

            public Dictionary<string, string> Config { get; set; }
        }

        private class DummyProduct : ProductType
        {
            public int Foo { get; set; }

            public int[] Bobs { get; set; }

            public DummyValue Dummy => new DummyValue(this);

            protected override ProductInstance Instantiate()
            {
                return new DummyProductInstance();
            }

            public ProductPartLink<DummyProduct> Part { get; set; }
        }

        private class DummyValue
        {
            public DummyValue(DummyProduct product)
            {
                Product = product;
            }

            /// <summary>
            /// Fake property to test shortcut resolution
            /// </summary>
            public DummyProduct Product { get; }
        }

        private class DummyProductInstance : ProductInstance<DummyProduct>, IIdentifiableObject
        {
            public IIdentity Identity { get; set; }

            public float Blah { get; set; }
        }
    }
}

