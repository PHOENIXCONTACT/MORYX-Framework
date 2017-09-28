using System.Data.Entity;
using Marvin.Model;
using Marvin.TestTools.TestMerge.Model;
using NUnit.Framework;

namespace Marvin.GeneratedModel.Tests
{
    [TestFixture]
    public class MergeInheritanceReferenceTests
    {
        private InMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());

            var parentFactory = new TestTools.Test.Model.InMemoryUnitOfWorkFactory();
            _unitOfWorkFactory = new InMemoryUnitOfWorkFactory { ParentFactory = parentFactory };
            parentFactory.Initialize();
            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void CollectionNotNull()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var child1Repo = uow.GetRepository<IMergedChildTPT1Repository>();
                
                // Act
                var child1 = child1Repo.Create(10, 10);

                // Assert
                Assert.NotNull(child1.MergedChild2s);

                uow.Save();

                child1.A = 10;
            }
        }
    }
}