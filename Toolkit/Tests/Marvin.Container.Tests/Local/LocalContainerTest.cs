using NUnit.Framework;

namespace Marvin.Container.Tests
{
    [TestFixture]
    public class LocalContainerTest
    {
        private LocalContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new LocalContainer();
            _container.ExecuteInstaller(new AutoInstaller(GetType().Assembly));
        }

        [Test]
        public void LocalsFound()
        {
            var local = _container.Resolve<ILocalComponent>();
            var localFac = _container.Resolve<ILocalFactory>();

            Assert.NotNull(local, "Failed to resolve local component!");
            Assert.NotNull(localFac, "Failed to resolve local factory");
        }

        [Test]
        public void GlobalsNotFound()
        {
            var global = _container.Resolve<GlobalComponent>();
            var model = _container.Resolve<ModelComponent>();

            Assert.Null(global, "Global component was registered in local container!");
            Assert.Null(model, "Global model was registered in local container!");
        }

        [Test]
        public void DefaultFactory()
        {
            var fac = _container.Resolve<ILocalFactory>();
            var comp = fac.Create();

            Assert.AreEqual(typeof(LocalComponent), comp.GetType(), "Factory not working!");
        }

        [Test]
        public void NameBasedFactory()
        {
            var fac = _container.Resolve<INamedComponentFactory>();
            var compA = fac.Create(NamedComponentA.ComponentName);
            var compB = fac.Create(NamedComponentB.ComponentName);

            Assert.AreEqual(NamedComponentA.ComponentName, compA.GetName(), "Component A not resolved by name!");
            Assert.AreEqual(NamedComponentB.ComponentName, compB.GetName(), "Component B not resolved by name!");
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