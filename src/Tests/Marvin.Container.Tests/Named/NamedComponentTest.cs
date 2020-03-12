// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Marvin.Container.Tests
{
    [TestFixture]
    public class NamedComponentTest
    {
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new CastleContainer();
            // Registerall involved components
            _container.Register<Component, Component>();
            _container.Register<Impossible, Impossible>();
            _container.Register<IDependency, DependencyA>("DepA", LifeCycle.Singleton);
            _container.Register<IDependency, DependencyB>("DepB", LifeCycle.Singleton);
        }
        
        [Test]
        public void UnnamedInjectsFirst()
        {
            var comp = _container.Resolve<Component>();

            Assert.NotNull(comp.Unnamed, "Container did not inject dependency");
            Assert.AreEqual("DepA", comp.Unnamed.GetName(), "Container did not inject default implementation");
        }

        [Test]
        public void InjectDepA()
        {
            var comp = _container.Resolve<Component>();

            Assert.NotNull(comp.DepA, "Container did not inject DependencyA");
            Assert.AreEqual("DepA", comp.DepA.GetName(), "Container did not inject default implementation");
        }

        [Test]
        public void InjectDepB()
        {
            var comp = _container.Resolve<Component>();

            Assert.NotNull(comp.DepB, "Container did not inject DependencyB");
            Assert.AreEqual("DepB", comp.DepB.GetName(), "Container did not inject default implementation");
        }
    }
}
