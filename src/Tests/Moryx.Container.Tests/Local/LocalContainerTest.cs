// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Moryx.Container.Tests
{
    [TestFixture]
    public class LocalContainerTest
    {
        private CastleContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new CastleContainer(new Dictionary<Type, string>());
            _container.LoadFromAssembly(GetType().Assembly);
        }

        [Test]
        public void LocalsFound()
        {
            var local = _container.Resolve<ILocalComponent>();
            var localFac = _container.Resolve<ILocalFactory>();

            Assert.That(local, Is.Not.Null, "Failed to resolve local component!");
            Assert.That(localFac, Is.Not.Null, "Failed to resolve local factory");
        }

        [Test]
        public void DefaultFactory()
        {
            var fac = _container.Resolve<ILocalFactory>();
            var comp = fac.Create();

            Assert.That(comp.GetType(), Is.EqualTo(typeof(LocalComponent)), "Factory not working!");
        }

        [Test]
        public void NameBasedFactory()
        {
            var fac = _container.Resolve<INamedComponentFactory>();
            var compA = fac.Create(NamedComponentA.ComponentName);
            var compB = fac.Create(NamedComponentB.ComponentName);

            Assert.That(compA.GetName(), Is.EqualTo(NamedComponentA.ComponentName), "Component A not resolved by name!");
            Assert.That(compB.GetName(), Is.EqualTo(NamedComponentB.ComponentName), "Component B not resolved by name!");
        }

        [Test]
        public void SpecificDependencyRegistration()
        {

        }

        [TearDown]
        public void Destroy()
        {
            _container.Destroy();
        }
    }
}
