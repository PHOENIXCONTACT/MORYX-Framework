using System;
using System.Data.Entity;
using System.Reflection;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;

namespace Marvin.Model.Tests
{
    public class UnitOfWorkFactoryTests
    {
        private UnitOfWorkFactoryMock _unitOfWorkFactory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _unitOfWorkFactory = new UnitOfWorkFactoryMock("Test");
            _unitOfWorkFactory.Initialize();
        }
        
        [TestCase(typeof(ICarEntityRepository), true, 
            Description = "Resolves an repository which is registered by API only.")]
        [TestCase(typeof(ISportCarRepository), true, 
            Description = "Resolves an repository which is registered by API and impl. Proxy is enabled.")]
        [TestCase(typeof(IHouseEntityRepository), false, 
            Description = "Resolves an repository which is registered by API and impl. Proxy is disabled.")]
        public void RepositoryRegistration(Type repositoryApi, bool shouldProxy)
        {
            // Arrange
            var unitOfWork = _unitOfWorkFactory.Create();
            var uow = (IGenericUnitOfWork)unitOfWork;

            // Act
            var repo = uow.GetRepository(repositoryApi);
            
            // Assert
            Assert.AreEqual(shouldProxy, repo.GetType().Name.EndsWith("Proxy"));

            // Cleanup
            unitOfWork.Dispose();
        }

        [TestCase(ContextMode.AllOn, true, true, true, Description = "All features enabled in DbConfiguration.")]
        [TestCase(ContextMode.AllOff, false, false, false, Description = "All features disabled in DbConfiguration.")]
        [TestCase(ContextMode.ProxyOnly, true, false, false, Description = "Only the proxy creation is enabled in DbConfiguration.")]
        [TestCase(ContextMode.ChangeTracking, true, false, true, Description = "ChangeTracking and also proxy is enabled in DbConfiguration.")]
        [TestCase(ContextMode.LazyLoading, true, true, false, Description = "LazyLoading and also proxy is enabled in DbConfiguration.")]
        public void ContextModes(ContextMode mode, bool isProxy, bool isLazy, bool isTracking)
        {
            // Arrange
            var contextProp = typeof(UnitOfWork).GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var allOn = _unitOfWorkFactory.Create(mode);

            // Assert
            var context = (DbContext)contextProp.GetValue(allOn);
            var configuration = context.Configuration;

            Assert.AreEqual(isProxy, configuration.ProxyCreationEnabled);
            Assert.AreEqual(isTracking, configuration.AutoDetectChangesEnabled);
            Assert.AreEqual(isLazy, configuration.LazyLoadingEnabled);
        }
    }
}
