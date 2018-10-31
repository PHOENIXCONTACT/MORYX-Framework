using System;
using System.Collections.Generic;
using Marvin.Bindings;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
{
    [TestFixture]
    public class ParameterBindingTests
    {
        [TestCase("Article.Product", typeof(DummyProduct))]
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
            Assert.AreEqual(resultType, result.GetType(), "Wrong object resolved");
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
            var resolver = new ProcessBindingResolverFactory().Create("Article.Product.Dummy.Product.Name");
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
            Assert.AreEqual("Bob", result);
            Assert.LessOrEqual(newCount, oldCount);
            Assert.AreEqual(8, oldCount);
            Assert.AreEqual(4, newCount);
        }

        public class InsertPartParameters : ParametersBase
        {
            public string Part { get; set; }  // e.g. Product.Part.Product

            public IProduct Target { get; set; }

            private IBindingResolver _resolver;
            protected override ParametersBase ResolveBinding(IProcess process)
            {
                if (_resolver == null)
                    _resolver = ResolverFactory.Create(Part);

                return new InsertPartParameters
                {
                    Target = (IProduct)_resolver.Resolve(process)
                };
            }
        }

        private static IProcess DummyProcess()
        {
            var product = new DummyProduct()
            {
                Id = 42,
                Name = "Bob",
                Foo = 1337,
                Bobs = new[] { 1, 3, 7, 42, 1337 },
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
                Article = product.CreateInstance()
            };
            process.Article.Id = 42;
            process.Article.Reworked = true;
            process.Article.State = ArticleState.Success;
            process.Article.Identity = new ProductIdentity("Test", 1);
            ((DummyArticle)process.Article).Blah = (float)0.815;

            return process;
        }

        private class DummyRecipe : ProductRecipe
        {
            public string OrderNumber { get; set; }

            public Dictionary<string, string> Config { get; set; }
        }

        private class DummyProduct : Product
        {
            public override string Type
            {
                get { return "DummyProduct"; }
            }

            public int Foo { get; set; }

            public int[] Bobs { get; set; }

            public DummyValue Dummy => new DummyValue(this);

            protected override Article Instantiate()
            {
                return new DummyArticle();
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

        private class DummyArticle : Article<DummyProduct>
        {
            ///
            public override string Type
            {
                get { return "DummyArticle"; }
            }

            public float Blah { get; set; }
        }
    }
}
