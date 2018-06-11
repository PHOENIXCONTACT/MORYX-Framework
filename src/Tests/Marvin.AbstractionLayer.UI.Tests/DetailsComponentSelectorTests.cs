using System;
using Marvin.Container;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.UI.Tests
{
    [TestFixture]
    public class DetailsComponentSelectorTests
    {
        private IContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new LocalContainer();

            _container.Register<DetailsComponentSelector, DetailsComponentSelector>();
            _container.Register<IInteractionController, InteractionControllerMock>();

            //Register component factory
            _container.Register<IDetailsComponentFactory>();
        }

        [Test]
        public void CreateDefault()
        {
            //Assert
            var factory = AddSingleComponent<DefaultDetailsMock>();

            //Act
            var def = factory.Create();

            //Assert
            AssertComponent(def, typeof(DefaultDetailsMock));
        }

        [Test]
        public void CreateUnknown()
        {
            //Assert
            var factory = AddSingleComponent<DefaultDetailsMock>();

            //Act
            var def = factory.Create("Unknown");

            //Assert
            AssertComponent(def, typeof(DefaultDetailsMock));
        }

        [Test]
        public void CreateSpecialized()
        {
            //Assert
            var factory = AddSingleComponent<DetailsComponentMock>();

            //Act
            var special = factory.Create(DetailsComponentMock.TypeName);

            //Assert
            AssertComponent(special, typeof(DetailsComponentMock));
        }

        private IDetailsComponentFactory AddSingleComponent<T>() where T : IDetailsComponent
        {
            _container.Register<IDetailsComponent, T>();
            return _container.Resolve<IDetailsComponentFactory>();
        }

        private static void AssertComponent(IDetailsComponent component, Type expectedClassType)
        {
            Assert.NotNull(component, "Component not found");
            Assert.IsInstanceOf(expectedClassType, component, "Wrong component loaded");
            Assert.IsTrue(component.InitializeCalled, "Initialize not called");
        }
    }
}