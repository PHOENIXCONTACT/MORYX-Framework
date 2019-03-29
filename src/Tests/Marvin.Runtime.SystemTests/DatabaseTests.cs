using System;
using System.Threading;
using Marvin.Model;
using Marvin.Runtime.Maintenance.Plugins.Databases;
using Marvin.Runtime.Modules;
using Marvin.TestTools.SystemTest;
using Marvin.TestTools.Test.Inheritance.Model;
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
            _databaseName = GetType().Name;
            _databaseConfigModel = HeartOfGoldController.CreateDatabaseConfig("localhost", _databaseName, "postgres", "postgres");

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 60
            };

            Console.WriteLine("Starting HeartOfGold");

            var started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.CreateClients();

            var result = _hogController.WaitForService("Maintenance", ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'Maintenance' did not reach state 'Running'");

            if (_hogController.CheckDatabase(_databaseConfigModel, InheritedTestModelConstants.Name).Result == TestConnectionResult.Success)
            {
                result = _hogController.DeleteDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);
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
            var checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);

            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, checkDatabase.Result, "Database '{0}' seems to exist.", _databaseName);

            var createDatabase = _hogController.CreateDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);

            Assert.IsTrue(createDatabase, "Can't create database '{0}'.", _databaseName);

            checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);
            Assert.AreEqual(TestConnectionResult.Success, checkDatabase.Result, "Database '{0}' does not to exist.", _databaseName);

            var deleteDatabase = _hogController.DeleteDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);

            Assert.IsTrue(deleteDatabase, "Can't delete database '{0}'.", _databaseName);

            checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, InheritedTestModelConstants.Name);
            
            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, checkDatabase.Result, "Database '{0}' seems to still exist.", _databaseName);
        }
    }
}