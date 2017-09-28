using System.Data.Entity;
using Marvin.Model;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;

namespace Marvin.GeneratedModel.Tests
{
    [TestFixture]
    public class InheritanceAndReferenceTests
    {
        private InMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());

            _unitOfWorkFactory = new InMemoryUnitOfWorkFactory();

            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void CollectionsNotNull()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var aRepo = uow.GetRepository<IARepository>();

                // Act
                var a = aRepo.Create(10);
                uow.Save();
                
                // Assert
                Assert.NotNull(a.Bs);
            }
        }

        [Test]
        public void CollectionUsed()
        {
            long id;
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var aRepo = uow.GetRepository<IARepository>();
                var bRepo = uow.GetRepository<IBRepository>();

                // Act
                var a = aRepo.Create(10);
                a.Bs.Add(bRepo.Create("Thomas"));
                a.Bs.Add(bRepo.Create("John"));

                uow.Save();

                id = a.Id;
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var a = uow.GetRepository<IARepository>().GetByKey(id);

                Assert.AreEqual(2, a.Bs.Count);
            }
        }
    }
}