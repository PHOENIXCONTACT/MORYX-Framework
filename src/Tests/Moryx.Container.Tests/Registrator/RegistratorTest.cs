// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.MicroKernel;
using NUnit.Framework;

namespace Moryx.Container.Tests
{
    [TestFixture]
    public class RegistratorTest
    {
        private CastleContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new CastleContainer();
            _container.Register<NamedDummy, NamedDummy>();
            _container.Register<UnnamedDummy, UnnamedDummy>();
        }

        [Test]
        public void DoubleRegistrationWithRegister()
        {
            Assert.Throws<ComponentRegistrationException>(() => _container.Register<NamedDummy, NamedDummy>());
            Assert.Throws<ComponentRegistrationException>(() => _container.Register<UnnamedDummy, UnnamedDummy>());
        }

        [Test]
        public void DoubleRegistrationWithAutoInstaller()
        {
            var fakeAutoInstaller = new FakeAutoInstaller();
            _container.ExecuteInstaller(fakeAutoInstaller);

            Assert.True(fakeAutoInstaller.SkippedNamed, "Named was not skipped");
            Assert.True(fakeAutoInstaller.SkippedUnnamed, "Unnamed was not skipped");
        }
    }
}
