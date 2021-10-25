// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Configuration;

#if USE_WCF
using System.ServiceModel;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{

    [Plugin(LifeCycle.Transient, typeof(IDatabaseMaintenance))]
#if USE_WCF
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class DatabaseMaintenance : IDatabaseMaintenance
#else
    [ApiController, Route(Endpoint), Produces("application/json")]
    [Endpoint(Name = nameof(IDatabaseMaintenance), Version = "3.0.0")]
    internal class DatabaseMaintenance : Controller, IDatabaseMaintenance
#endif
    {
        internal const string Endpoint = "databases";

        #region Dependencies

        /// <summary>
        /// Global component for database contexts
        /// </summary>
        public IDbContextManager DbContextManager { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("DatabaseMaintenance")]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Configuration for the database plugin
        /// </summary>
        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields and Properties

        private DatabaseConfig DatabaseConfig => Config.Plugins.OfType<DatabaseConfig>().FirstOrDefault();

        #endregion

#if !USE_WCF
        [HttpGet]
#endif
        public async Task<DataModel[]> GetAll()
        {
            var contexts = DbContextManager.Contexts.ToArray();
            var dataModels = new DataModel[contexts.Length];

            for (var i = 0; i < contexts.Length; i++)
                dataModels[i] = await Convert(contexts[i]);

            return dataModels;
        }

#if !USE_WCF
        [HttpGet("model/{targetModel}")]
#endif
        public Task<DataModel> GetModel(string targetModel)
        {
            return Convert(DbContextManager.Contexts.FirstOrDefault(context => TargetModelName(context) == targetModel));
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/config")]
#endif
        public void SetDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return;

            // Save config and reload all DataModels
            UpdateConfigFromModel(match.Config, config);
            match.UpdateConfig();

        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/config/test")]
#endif
        public async Task<TestConnectionResponse> TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new TestConnectionResponse { Result = TestConnectionResult.ConfigurationError };

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var result = await targetConfigurator.TestConnection(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

#if !USE_WCF
        [HttpPost("createall")]
#endif
        public async Task<InvocationResponse> CreateAll()
        {
            var bulkResult = await BulkOperation(mc => mc.CreateDatabase(mc.Config), "Creation");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/create")]
#endif
        public async Task<InvocationResponse> CreateDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            try
            {
                var creationResult = await targetConfigurator.CreateDatabase(updatedConfig);
                return creationResult
                    ? new InvocationResponse()
                    : new InvocationResponse("Cannot create database. May be the database already exists or was misconfigured.");
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database creation failed!");
                return new InvocationResponse(ex);
            }
        }

#if !USE_WCF
        [HttpDelete("/")]
#endif
        public async Task<InvocationResponse> EraseAll()
        {
            var bulkResult = await BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

#if !USE_WCF
        [HttpDelete("model/{targetModel}")]
#endif
        public async Task<InvocationResponse> EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            try
            {
                await targetConfigurator.DeleteDatabase(updatedConfig);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database deletion failed!");
                return new InvocationResponse(ex);
            }
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/dump")]
#endif
        public async Task<InvocationResponse> DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);

            var targetPath = Path.Combine(DatabaseConfig.SetupDataDir, targetModel);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            await targetConfigurator.DumpDatabase(updatedConfig, targetPath);

            return new InvocationResponse();
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/restore")]
#endif
        public async Task<InvocationResponse> RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            var filePath = Path.Combine(DatabaseConfig.SetupDataDir, targetModel, request.BackupFileName);
            await targetConfigurator.RestoreDatabase(updatedConfig, filePath);

            return new InvocationResponse();
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/migrate")]
#endif
        public async Task<DatabaseMigrationSummary> MigrateDatabase(string targetModel, DatabaseConfigModel configModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
            {
                return new DatabaseMigrationSummary
                {
                    Result = MigrationResult.Error,
                    ExecutedMigrations = Array.Empty<string>()
                };
            }

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            return await targetConfigurator.MigrateDatabase(config);
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/setup")]
#endif
        public async Task<InvocationResponse> ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            var contextType = DbContextManager.Contexts.First(c => TargetModelName(c) == targetModel);
            var targetConfigurator = DbContextManager.GetConfigurator(contextType);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);

            var setupExecutor = DbContextManager.GetSetupExecutor(contextType);

            var targetSetup = setupExecutor.GetAllSetups().FirstOrDefault(s => s.GetType().FullName == request.Setup.Fullname);
            if (targetSetup == null)
                return new InvocationResponse("No matching setup found");

            // Provide logger for model
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (targetSetup is ILoggingComponent loggingComponent)
                loggingComponent.Logger = Logger.GetChild("Setup", loggingComponent.GetType());

            try
            {
                await setupExecutor.Execute(config, targetSetup, request.Setup.SetupData);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database setup execution failed!");
                return new InvocationResponse(ex);
            }
        }

        private SetupModel ConvertSetup(IModelSetup setup)
        {
            var model = new SetupModel
            {
                Fullname = setup.GetType().FullName,
                SortOrder = setup.SortOrder,
                Name = setup.Name,
                Description = setup.Description,
                SupportedFileRegex = setup.SupportedFileRegex
            };
            return model;
        }

        private IModelConfigurator GetTargetConfigurator(string model)
        {
            var context = DbContextManager.Contexts.First(c => TargetModelName(c) == model);
            return DbContextManager.GetConfigurator(context);
        }

        private async Task<DataModel> Convert(Type contextType)
        {
            var configurator = DbContextManager.GetConfigurator(contextType);
            if (configurator?.Config == null)
            {
                return null;
            }

            var dbConfig = configurator.Config;
            var model = new DataModel
            {
                TargetModel = TargetModelName(contextType),
                Config = new DatabaseConfigModel
                {
                    Server = dbConfig.Host,
                    Port = dbConfig.Port,
                    Database = dbConfig.Database,
                    User = dbConfig.Username,
                    Password = dbConfig.Password
                },
                Setups = GetAllSetups(contextType),
                Backups = GetAllBackups(contextType),
                AvailableMigrations = await GetAvailableMigrations(dbConfig, configurator),
                AppliedMigrations = await GetInstalledMigrations(dbConfig, configurator)
            };
            return model;
        }

        private SetupModel[] GetAllSetups(Type contextType)
        {
            var setupExecutor = DbContextManager.GetSetupExecutor(contextType);
            var allSetups = setupExecutor.GetAllSetups();
            var setups = allSetups.Where(setup => string.IsNullOrEmpty(setup.SupportedFileRegex))
                                  .Select(ConvertSetup).OrderBy(setup => setup.SortOrder).ToList();
            string[] files;
            if (!Directory.Exists(DatabaseConfig.SetupDataDir) || !(files = Directory.GetFiles(DatabaseConfig.SetupDataDir)).Any())
                return setups.ToArray();

            var fileSetups = allSetups.Where(setup => !string.IsNullOrEmpty(setup.SupportedFileRegex))
                                      .Select(ConvertSetup).ToList();
            foreach (var setup in fileSetups)
            {
                var regex = new Regex(setup.SupportedFileRegex);
                var matchingFiles = files.Where(file => regex.IsMatch(Path.GetFileName(file)));
                setups.AddRange(matchingFiles.Select(setup.CopyWithFile));
            }
            return setups.OrderBy(setup => setup.SortOrder).ToArray();
        }

        private BackupModel[] GetAllBackups(Type contextType)
        {
            var targetModel = TargetModelName(contextType);
            var backupFolder = Path.Combine(DatabaseConfig.SetupDataDir, targetModel);

            if (!Directory.Exists(backupFolder))
                return new BackupModel[0];

            var allBackups = Directory.EnumerateFiles(backupFolder, "*.backup").ToList();
            var backups = from backup in allBackups
                let fileName = Path.GetFileName(backup)
                let fileInfo = new FileInfo(backup)
                select new BackupModel
                {
                    FileName = fileName,
                    Size = (int) fileInfo.Length / 1024,
                    CreationDate = fileInfo.CreationTime
                };

            return backups.ToArray();
        }

        private static async Task<DbMigrationsModel[]> GetAvailableMigrations(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var availableMigrations = await configurator.AvailableMigrations(dbConfig);
            return availableMigrations.Select(m => new DbMigrationsModel {Name = m}).ToArray();
        }

        private static async Task<DbMigrationsModel[]> GetInstalledMigrations(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var appliedMigrations = await configurator.AppliedMigrations(dbConfig);
            return appliedMigrations.Select(m => new DbMigrationsModel {Name = m}).ToArray();
        }

        private static IDatabaseConfig UpdateConfigFromModel(IDatabaseConfig dbConfig, DatabaseConfigModel model)
        {
            dbConfig.Host = model.Server;
            dbConfig.Port = model.Port;
            dbConfig.Database = model.Database;
            dbConfig.Username = model.User;
            dbConfig.Password = model.Password;
            return dbConfig;
        }

        private static string TargetModelName(Type contextType) => contextType.FullName;

        private async Task<string> BulkOperation(Func<IModelConfigurator, Task> operation, string operationName)
        {
            var result = string.Empty;
            foreach (var contextType in DbContextManager.Contexts)
            {
                var configurator = DbContextManager.GetConfigurator(contextType);
                try
                {
                    await operation(configurator);
                }
                catch (Exception ex)
                {
                    Logger.LogException(LogLevel.Warning, ex, "{0} of {1} failed!", operationName, TargetModelName(contextType));
                    result += $"{operationName} of {TargetModelName(contextType)} failed!\n";
                }
            }
            return result;
        }
    }
}
