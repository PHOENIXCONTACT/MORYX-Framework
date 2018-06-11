using Caliburn.Micro;
using Marvin.Container;
using Marvin.Logging;
using Marvin.TestTools.UnitTest;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.UI.Tests
{
    [TestFixture]
    public class MasterDetailsWorkspaceTests
    {
        private MasterDetailsWorkspaceMock _workspace;
        private IContainer _container;
        private IDetailsComponentFactory _componentFactory;

        [SetUp]
        public void SetUp()
        {
            _container = new LocalContainer();
            _container.Register<IModuleLogger, DummyLogger>();

            _container.Register<DetailsComponentSelector, DetailsComponentSelector>();
            _container.Register<IInteractionController, InteractionControllerMock>();

            //Register component factory
            _container.Register<IDetailsComponentFactory>();

            _container.Register<IDetailsComponent, DefaultDetailsMock>();
            _container.Register<IDetailsComponent, EmptyDetailsMock>();
            _container.Register<IDetailsComponent, DetailsComponentMock>();

            _componentFactory = _container.Resolve<IDetailsComponentFactory>();

            _workspace = new MasterDetailsWorkspaceMock
            {
                DetailsFactory = _componentFactory,
                DialogManager = new DialogManagerMock(),
                Logger = new DummyLogger()
            };

            ScreenExtensions.TryActivate(_workspace);
        }

        [Test]
        public void ActivateItem()
        {
            // Arrange
            var newDetails = _componentFactory.Create(DetailsComponentMock.TypeName);

            // Act
            _workspace.ActivateItem(newDetails);

            // Assert
            Assert.AreEqual(newDetails, _workspace.ActiveItem);
        }

        /// <summary>
        /// Will set the busy mode within the details.
        /// The attached events should be raised and the master should know about the changes
        /// </summary>
        [Test]
        public void CheckEventBusy()
        {
            // Arrange
            var newDetails = ActivateDefault();

            // Act
            newDetails.SetBusy(true);

            // Assert
            Assert.IsTrue(_workspace.IsDetailsInBusyMode);

            // Act
            newDetails.SetBusy(false);

            // Assert
            Assert.IsFalse(_workspace.IsDetailsInBusyMode);
        }

        /// <summary>
        /// Will set the edit mode within the details.
        /// The attached events should be raised and the master should know about the changes
        /// </summary>
        [Test]
        public void CheckEventEditMode()
        {
            // Arrange
            var newDetails = ActivateDefault();

            // Act
            newDetails.EnterEditMode();

            // Assert
            Assert.IsTrue(_workspace.IsDetailsInEditMode);
        }

        private DetailsComponentMock ActivateDefault()
        {
            var newDetails = (DetailsComponentMock)_componentFactory.Create(DetailsComponentMock.TypeName);
            _workspace.ActivateItem(newDetails);

            return newDetails;
        }
    }
}
