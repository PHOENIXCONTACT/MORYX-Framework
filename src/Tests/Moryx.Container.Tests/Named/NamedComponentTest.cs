// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Moryx.Container.Tests
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

            Assert.That(comp.Unnamed, Is.Not.Null, "Container did not inject dependency");
            Assert.That(comp.Unnamed.GetName(), Is.EqualTo("DepA"), "Container did not inject default implementation");
        }

        [Test]
        public void InjectDepA()
        {
            var comp = _container.Resolve<Component>();

            Assert.That(comp.DepA, Is.Not.Null, "Container did not inject DependencyA");
            Assert.That(comp.DepA.GetName(), Is.EqualTo("DepA"), "Container did not inject default implementation");
        }

        [Test]
        public void InjectDepB()
        {
            var comp = _container.Resolve<Component>();

            Assert.That(comp.DepB, Is.Not.Null, "Container did not inject DependencyB");
            Assert.That(comp.DepB.GetName(), Is.EqualTo("DepB"), "Container did not inject default implementation");
        }
    }
}
