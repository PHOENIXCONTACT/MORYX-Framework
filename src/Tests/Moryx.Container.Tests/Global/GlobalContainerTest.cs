// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Moryx.Container.Tests
{
    [TestFixture]
    public class GlobalContainerTest
    {
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            var container = new GlobalContainer();
            container.ExecuteInstaller(new AutoInstaller(GetType().Assembly));
            _container = container;
        }

        [Test]
        public void LocalsNotFound()
        {
            var local = _container.Resolve<LocalComponent>();
            var localFac = _container.Resolve<ILocalFactory>();

            Assert.Null(local, "Local component was registered in global container!");
            Assert.Null(localFac, "Local factory was registered in global container");
        }

        [Test]
        public void GlobalsFound()
        {
            var global = _container.Resolve<GlobalComponent>();
            var model = _container.Resolve<ModelComponent>();

            Assert.NotNull(global, "Global component was not registered!");
            Assert.NotNull(model, "Global model was not registered!");
        }

        [TearDown]
        public void Destroy()
        {
            _container.Destroy();
        }
    }
}
