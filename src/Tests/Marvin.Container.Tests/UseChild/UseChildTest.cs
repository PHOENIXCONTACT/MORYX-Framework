using NUnit.Framework;

namespace Marvin.Container.Tests
{
    [TestFixture]
    public class UseChildSingleParentTest
    {
        private Parent _parent;
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new CastleContainer();
            // Register only the parent
            _parent = new Parent("Parent");
            _container.SetInstance(_parent);
        }

        [Test]
        public void InjectParent()
        {
            // Arrange: Register component
            _container.Register<ParentOnly, ParentOnly>();

            // Act: Resolve component
            var comp = _container.Resolve<ParentOnly>();

            // Assert: Check if correct instance was set
            Assert.AreEqual(_parent, comp.Parent, "Container did not inject the right instance!");
        }

        [Test]
        public void InjectTypedChild()
        {
            // Arrange: Register component
            _container.Register<TypedChild, TypedChild>();

            // Act: Resolve component
            var comp = _container.Resolve<TypedChild>();

            // Assert: Check if type was set
            Assert.AreEqual(comp.GetType(), comp.Child.Target, "Container did not forward type corretly!");
        }

        [Test]
        public void InjectNamedChild()
        {
            // Arrange: Register component
            _container.Register<NamedChild, NamedChild>();

            // Act: Resolve component
            var comp = _container.Resolve<NamedChild>();

            // Assert: Check if type and name were set
            Assert.AreEqual(comp.GetType(), comp.Child.Target, "Container did not forward type corretly!");
            Assert.AreEqual("Child", comp.Child.Name, "Container did not forward type corretly!");
        }

        [TearDown]
        public void TearDown()
        {
            _container.Destroy();
            _container = null;
        }
    }

    [TestFixture]
    public class UseChildMultiParentTests
    {
        private Parent _parent1;
        private Parent _parent2;
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            _container = new CastleContainer();

            // Register the parents
            _parent1 = new Parent("Parent1");
            _container.SetInstance(_parent1, "Parent1");
            _parent2 = new Parent("Parent2");
            _container.SetInstance(_parent2, "Parent2");
        }

        [Test]
        public void InjectParent()
        {
            // Arrange: Register component
            _container.Register<NamedParentOnly, NamedParentOnly>("Test", LifeCycle.Singleton);

            // Act: Resolve component
            var comp = _container.Resolve<NamedParentOnly>();

            // Assert: Check if correct instance was set
            Assert.AreEqual(_parent2, comp.Parent, "Container did not inject the right instance!");
        }

        [Test]
        public void InjectNamedChild()
        {
            // Arrange: Register component
            _container.Register<NamedChildNamedParent, NamedChildNamedParent>("Test", LifeCycle.Singleton);

            // Act: Resolve component
            var comp = _container.Resolve<NamedChildNamedParent>();

            // Assert: Check if type and name were set
            Assert.AreEqual(comp.GetType(), comp.Child.Target, "Container did not forward type corretly!");
            Assert.AreEqual(_parent2, comp.Child.Parent, "Container did not fetch from the correct parent!");
            Assert.AreEqual("Child", comp.Child.Name, "Container did not forward name correctly!");
        }
    }
}
