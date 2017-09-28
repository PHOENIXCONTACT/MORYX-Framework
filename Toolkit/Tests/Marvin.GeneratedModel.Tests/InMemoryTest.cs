using System;
using System.Data.Entity;
using Marvin.Model;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;
using HugePoco = Marvin.TestTools.Test.Model.HugePoco;
using IHugePocoRepository = Marvin.TestTools.Test.Model.IHugePocoRepository;

namespace Marvin.GeneratedModel.Tests
{
    [TestFixture]
    public class InMemoryTest
    {
        private InMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            DbConfiguration.SetConfiguration(new EntityFrameworkConfiguration());

            _unitOfWorkFactory = new InMemoryUnitOfWorkFactory();
            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void DifferentInMemoryDatabases()
        {
            //Arrange
            var factoryA = new InMemoryUnitOfWorkFactory("A");
            factoryA.Initialize();

            var factoryB = new InMemoryUnitOfWorkFactory("B");
            factoryB.Initialize();

            //Act
            using (var uow = factoryA.Create())
            {
                uow.GetRepository<IHugePocoRepository>().Create(1.0, 2, 3.0, 4, 5.0, 6, 7.0, 8, 9.0, 10);
                uow.Save();
            }

            //Assert
            using (var uow = factoryB.Create())
            {
                var pocos = uow.GetRepository<IHugePocoRepository>().GetAll();
                Assert.IsEmpty(pocos);
            }
        }

        [Test]
        public void InsertReadUpdateDelete()
        {
            long id;

            using (var uow = _unitOfWorkFactory.Create(ContextMode.ProxyTracking))
            {
                var repository = uow.GetRepository<IHugePocoRepository>();
                var poco = repository.Create(1.0, 2, 3.0, 4, 5.0, 6, 7.0, 8, 9.0, 10);

                uow.Save();

                id = poco.Id;
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var repository = uow.GetRepository<IHugePocoRepository>();
                var poco = repository.GetByKey(id);

                Assert.AreEqual(1.0, poco.Float1);
                Assert.AreEqual(2,   poco.Number1);
                Assert.AreEqual(3.0, poco.Float2);
                Assert.AreEqual(4,   poco.Number2);
                Assert.AreEqual(5.0, poco.Float3);
                Assert.AreEqual(6,   poco.Number3);
                Assert.AreEqual(7.0, poco.Float4);
                Assert.AreEqual(8,   poco.Number4);
                Assert.AreEqual(9.0, poco.Float5);
                Assert.AreEqual(10,  poco.Number5);

                Assert.IsNull(poco.Name1);
                Assert.IsNull(poco.Name2);
                Assert.IsNull(poco.Name3);
                Assert.IsNull(poco.Name4);
                Assert.IsNull(poco.Name5);

                poco.Name1 = "ASDF";
                poco.Name2 = "QWERTZ";
                poco.Name3 = "12345";
                poco.Name4 = "YXCVB";
                poco.Name5 = "!\"§$%";

                uow.Save();

                Assert.AreEqual(id, poco.Id, "Id of database record changed.");
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var repository = uow.GetRepository<IHugePocoRepository>();
                var poco = repository.GetByKey(id);

                Assert.AreEqual(1.0, poco.Float1);
                Assert.AreEqual(2, poco.Number1);
                Assert.AreEqual(3.0, poco.Float2);
                Assert.AreEqual(4, poco.Number2);
                Assert.AreEqual(5.0, poco.Float3);
                Assert.AreEqual(6, poco.Number3);
                Assert.AreEqual(7.0, poco.Float4);
                Assert.AreEqual(8, poco.Number4);
                Assert.AreEqual(9.0, poco.Float5);
                Assert.AreEqual(10, poco.Number5);


                Assert.AreEqual(31, poco.Name1.Length);
                Assert.AreEqual(31, poco.Name2.Length);
                Assert.AreEqual(31, poco.Name3.Length);
                Assert.AreEqual(31, poco.Name4.Length);
                Assert.AreEqual(31, poco.Name5.Length);

                Assert.AreEqual("ASDF",   poco.Name1.Trim());
                Assert.AreEqual("QWERTZ", poco.Name2.Trim());
                Assert.AreEqual("12345" , poco.Name3.Trim());
                Assert.AreEqual("YXCVB" , poco.Name4.Trim());
                Assert.AreEqual("!\"§$%", poco.Name5.Trim());

                repository.Remove(poco);

                uow.Save();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var repository = uow.GetRepository<IHugePocoRepository>();
                var poco = repository.GetByKey(id);

                Assert.IsNull(poco);
            }

        }
    }

}