// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using Moryx.Model;
using Moryx.Runtime.Maintenance.Databases;
using Moryx.Runtime.Modules;
using Moryx.TestTools.SystemTest;
using Moryx.TestTools.Test.Model;
using NUnit.Framework;

namespace Moryx.Runtime.SystemTests
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
        private string _targetModel;

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

            _targetModel = typeof(TestModelContext).FullName;

            Console.WriteLine("Starting HeartOfGold");

            var started = _hogController.StartHeartOfGold();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            _hogController.CreateClients();

            var result = _hogController.WaitForService("Maintenance", ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'Maintenance' did not reach state 'Running'");

            if (_hogController.CheckDatabase(_databaseConfigModel, _targetModel).Result == TestConnectionResult.Success)
            {
                result = _hogController.DeleteDatabase(_databaseConfigModel, _targetModel);
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
            var checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, _targetModel);

            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, checkDatabase.Result, "Database '{0}' seems to exist.", _databaseName);

            var createDatabase = _hogController.CreateDatabase(_databaseConfigModel, _targetModel);

            Assert.IsTrue(createDatabase, "Can't create database '{0}'.", _databaseName);

            checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, _targetModel);
            Assert.AreEqual(TestConnectionResult.Success, checkDatabase.Result, "Database '{0}' does not to exist.", _databaseName);

            var deleteDatabase = _hogController.DeleteDatabase(_databaseConfigModel, _targetModel);

            Assert.IsTrue(deleteDatabase, "Can't delete database '{0}'.", _databaseName);

            checkDatabase = _hogController.CheckDatabase(_databaseConfigModel, _targetModel);

            Assert.AreEqual(TestConnectionResult.ConnectionOkDbDoesNotExist, checkDatabase.Result, "Database '{0}' seems to still exist.", _databaseName);
        }
    }
}
