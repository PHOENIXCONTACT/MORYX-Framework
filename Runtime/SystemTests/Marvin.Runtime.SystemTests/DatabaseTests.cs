using System;
using System.Linq;
using System.Threading;
using Devart.Data.PostgreSql;
using Marvin.Model;
using Marvin.TestTools.SystemTest;
using Marvin.TestTools.SystemTest.DatabaseMaintenance;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;

namespace Marvin.Runtime.SystemTests
{
    /// <summary>
    /// These tests shall check two aspects: They shall verify the HoG functionality but also wether the HeartOfGoldController is working as expected.
    /// </summary>
    [TestFixture]
    public class BasicDatabaseTests : IDisposable
    {
        private HeartOfGoldController _hogController;
        private string _databaseName;
        private DatabaseConfigModel _databaseConfigModel;

        [SetUp]
        public void Setup()
        {
            HogHelper.CopyTestModels();

            _databaseName = GetType().Name;
            _databaseConfigModel = HeartOfGoldController.CreateDatabaseConfig("localhost", _databaseName, "postgres", "postgres");

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 60
            };

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.CreateClients();

            bool result = _hogController.WaitForService("Maintenance", ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");

            if (_hogController.CheckDatabase(_databaseConfigModel, TestModelConstants.Namespace))
            {
                result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);
                Assert.IsTrue(result, "Can't delete database '{0}' in Setup.", _databaseName);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.Process.Kill();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }

            HogHelper.DeleteTestModels();
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [Test]
        public void CreateDeleteDatabaseTest()
        {
            bool result = _hogController.CheckDatabase(_databaseConfigModel, TestModelConstants.Namespace);

            Assert.IsFalse(result, "Database '{0}' seems to exist.", _databaseName);

            result = _hogController.CreateDatabase(_databaseConfigModel, TestModelConstants.Namespace);

            Assert.IsTrue(result, "Can't create database '{0}'.", _databaseName);

            result = _hogController.CheckDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsTrue(result, "Database '{0}' does not to exist.", _databaseName);

            result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);

            Assert.IsTrue(result, "Can't delete database '{0}'.", _databaseName);

            result = _hogController.CheckDatabase(_databaseConfigModel, TestModelConstants.Namespace);

            Assert.IsFalse(result, "Database '{0}' seems to still exist.", _databaseName);
        }

        [Test]
        public void BackupRestoreDatabaseTest()
        {
            var result = _hogController.CreateDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsTrue(result, "Can't create database '{0}'.", _databaseName);
            
            var unitOfWorkFactory = HeartOfGoldController.CreateUnitOfWorkFactory<EntityFrameworkUnitOfWorkFactory, DbConfig>(_databaseConfigModel)
                                                         .Build<EntityFrameworkUnitOfWorkFactory>();
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                var huge = unitOfWork.GetRepository<IHugePocoRepository>().Create();
                huge.Name2 = "Dennis";

                unitOfWork.Save();
            }

            var dumpResult = _hogController.DumpDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsNotNull(dumpResult, "Can't dump database database '{0}'.", _databaseName);

            PgSqlConnection.ClearAllPools(true);

            Thread.Sleep(1000);

            result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsTrue(result, "Can't delete database '{0}'.", _databaseName);

            result = _hogController.CreateDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsTrue(result, "Can't create database '{0}'.", _databaseName);

            result = _hogController.RestoreDatabase(_databaseConfigModel, TestModelConstants.Namespace, dumpResult.DumpName);
            Assert.IsTrue(result, "Can't restore database '{0}'.", _databaseName);

            HugePoco dennis;
            using (var unitOfWork = unitOfWorkFactory.Create(ContextMode.AllOff))
            {
                dennis = unitOfWork.GetRepository<IHugePocoRepository>().Linq.FirstOrDefault(huge => huge.Name2.Equals("Dennis"));
            }

            Assert.IsNotNull(dennis, "Entity not found in restored database!");

            PgSqlConnection.ClearAllPools(true);

            Thread.Sleep(1000);

            result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);
            Assert.IsTrue(result, "Can't delete database '{0}'.", _databaseName);
        }
    }
}