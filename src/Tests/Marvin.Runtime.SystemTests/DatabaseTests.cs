using System;
using System.Linq;
using System.Threading;
using Devart.Data.PostgreSql;
using Marvin.Model;
using Marvin.Modules.Server;
using Marvin.Runtime.Modules;
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


        // Test does not work: If you find a solution, feel free to fix it.
        //Devart.Data.PostgreSql.PgSqlException (0x80004005): kann Funktion set_timestamps() nicht löschen, weil andere Objekte davon abhängen
        // bei Devart.Data.PostgreSql.PgSqlDataReader.f(Int32 A_0)
        // bei Devart.Data.PostgreSql.PgSqlCommand.InternalExecute(CommandBehavior behavior, IDisposable stmt, Int32 startRecord, Int32 maxRecords)
        // bei Devart.Common.DbCommandBase.InternalExecute(CommandBehavior behavior, IDisposable stmt, Int32 startRecord, Int32 maxRecords, Boolean nonQuery)
        // bei Devart.Common.DbCommandBase.ExecuteDbDataReader(CommandBehavior behavior, Boolean nonQuery)
        // bei Devart.Common.DbCommandBase.ExecuteDbDataReader(CommandBehavior behavior)
        // bei Devart.Common.DbCommandBase.ExecuteNonQuery()
        // bei Devart.Common.DbScript.ExecuteSqlStatement(SqlStatement sqlStatement, Boolean forceExecute)
        // bei Devart.Common.DbScript.a(SqlStatement A_0, Boolean A_1)
        // bei Devart.Common.DbScript.a(Boolean A_0, IDataReader& A_1)
        // bei Devart.Common.DbScript.Execute()
        // bei Devart.Data.PostgreSql.PgSqlDump.InternalRestore(TextReader reader)
        // bei Devart.Common.DbDump.b(String A_0)
        // bei Devart.Common.DbDump.Restore(String fileName)
        // bei Marvin.TestTools.Test.Model.ModelConfigurator.<>c__DisplayClass22_0.<RestoreDatabase>b__0(PgSqlDump pgDump)
        // bei Marvin.TestTools.Test.Model.ModelConfigurator.DatabaseDumpOperation(IDatabaseConfig config, Action`1 dumpOperation)
        // bei Marvin.TestTools.Test.Model.ModelConfigurator.RestoreDatabase(IDatabaseConfig config, String filePath)
        // bei Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf.DatabaseMaintenance.<>c__DisplayClass38_0.<RestoreDatabase>b__0(<>f__AnonymousType3`2 context) in C:\M2\M2\Svn\MarvinPlatform-2.7\Runtime\Modules\Marvin.Runtime.Maintenance\Plugins\DatabaseMaintenance\Wcf\DatabaseMaintenance.cs:Zeile 179.
        // bei Marvin.Tools.ParallelOperations.<>c__DisplayClass14_0`1.<ExecuteParallel>b__0(Object state) in C:\M2\M2\Svn\MarvinPlatform-2.7\Toolkit\PlatformCore\Marvin.PlatformTools\Tools\ParallelOperations.cs:Zeile 122.
        //[Test] 
        //public void BackupRestoreDatabaseTest()
        //{
        //    var result = _hogController.CreateDatabase(_databaseConfigModel, TestModelConstants.Namespace);
        //    Assert.IsTrue(result, "Can't create database '{0}'.", _databaseName);

        //    var unitOfWorkFactory = HeartOfGoldController.CreateUnitOfWorkFactory<EntityFrameworkUnitOfWorkFactory, DbConfig>(_databaseConfigModel)
        //                                                 .Build<EntityFrameworkUnitOfWorkFactory>();
        //    using (var unitOfWork = unitOfWorkFactory.Create())
        //    {
        //        var huge = unitOfWork.GetRepository<IHugePocoRepository>().Create();
        //        huge.Name2 = "Dennis";

        //        unitOfWork.Save();
        //    }

        //    var dumpResult = _hogController.DumpDatabase(_databaseConfigModel, TestModelConstants.Namespace);
        //    Assert.IsNotNull(dumpResult, "Can't dump database database '{0}'.", _databaseName);

        //    PgSqlConnection.ClearAllPools(true);

        //    Thread.Sleep(1000);

        //    result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);
        //    Assert.IsTrue(result, "Can't delete database '{0}'.", _databaseName);

        //    result = _hogController.CreateDatabase(_databaseConfigModel, TestModelConstants.Namespace);
        //    Assert.IsTrue(result, "Can't create database '{0}'.", _databaseName);

        //    result = _hogController.RestoreDatabase(_databaseConfigModel, TestModelConstants.Namespace, dumpResult.DumpName);
        //    Assert.IsTrue(result, "Can't restore database '{0}'.", _databaseName);

        //    HugePoco dennis;
        //    using (var unitOfWork = unitOfWorkFactory.Create(ContextMode.AllOff))
        //    {
        //        dennis = unitOfWork.GetRepository<IHugePocoRepository>().Linq.FirstOrDefault(huge => huge.Name2.Equals("Dennis"));
        //    }

        //    Assert.IsNotNull(dennis, "Entity not found in restored database!");

        //    PgSqlConnection.ClearAllPools(true);

        //    Thread.Sleep(1000);

        //    result = _hogController.DeleteDatabase(_databaseConfigModel, TestModelConstants.Namespace);
        //    Assert.IsTrue(result, "Can't delete database '{0}'.", _databaseName);
        //}
    }
}