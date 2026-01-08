// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
            Assert.DoesNotThrow(() => _container.Register<NamedDummy, NamedDummy>());
            Assert.DoesNotThrow(() => _container.Register<UnnamedDummy, UnnamedDummy>());
        }

        [Test]
        public void DoubleRegistrationWithAutoInstaller()
        {
            Assert.DoesNotThrow(() => _container.LoadFromAssembly(GetType().Assembly));
        }
    }
}
