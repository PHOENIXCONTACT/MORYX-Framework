// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Bindings;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class ParameterBindingTests
    {
        [TestCase("Article.ProductType", typeof(DummyType))]
        [TestCase("Recipe.Config[Foo].Length", typeof(int))]
        [TestCase("Product.Part.Product.Name", typeof(string))]
        [TestCase("Product.Part.Name", typeof(string), Description = "Use part link shortcut")]
        [TestCase("Product.Part.Product.Name", typeof(string))]
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

        [Test(Description = "Remove Product shortcut resolve if source is neither product instance nor Process")]
        public void ShortcutRemoval()
        {
            // Arrange
            var resolver = new ProcessBindingResolverFactory().Create("ProductInstance.Type.Dummy.ProductType.Name");
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
            public string Part { get; set; }  // e.g. Product.Part.Product

            public IProductType Target { get; set; }

            private IBindingResolver _resolver;

            protected override void Populate(IProcess process, Parameters instance)
            {
                var parameters = (InsertPartParameters) instance;

                if (_resolver == null)
                    _resolver = ResolverFactory.Create(Part);

                parameters.Target = (IProductType) _resolver.Resolve(process);
            }
        }

        private static IProcess DummyProcess()
        {
            var product = new DummyType()
            {
                Id = 42,
                Name = "Bob",
                Foo = 1337,
                Bobs = new[] { 1, 3, 7, 42, 1337 },
                Identity = new ProductIdentity("123456", 01),
                Part = new ProductPartLink<DummyType>(2)
                {
                    Product = new DummyType
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
            process.ProductInstance.Id = 42;
            ((DummyInstance)process.ProductInstance).Blah = (float)0.815;

            return process;
        }

        private class DummyRecipe : ProductRecipe
        {
            public string OrderNumber { get; set; }

            public Dictionary<string, string> Config { get; set; }
        }

        private class DummyType : ProductType
        {
            public int Foo { get; set; }

            public int[] Bobs { get; set; }

            public DummyValue Dummy => new DummyValue(this);

            protected override ProductInstance Instantiate()
            {
                return new DummyInstance();
            }

            public ProductPartLink<DummyType> Part { get; set; }
        }

        private class DummyValue
        {
            public DummyValue(DummyType type)
            {
                ProductType = type;
            }

            /// <summary>
            /// Fake property to test shortcut resolution
            /// </summary>
            public DummyType ProductType { get; }
        }

        private class DummyInstance : ProductInstance<DummyType>
        {
            public float Blah { get; set; }
        }
    }
}
