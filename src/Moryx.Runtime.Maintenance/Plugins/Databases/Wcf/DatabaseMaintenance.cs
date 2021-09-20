// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Configuration;
#if USE_WCF
using System.ServiceModel;
#else
using Microsoft.AspNetCore.Mvc;
#endif


namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
#if USE_WCF
    [Plugin(LifeCycle.Singleton, typeof(IDatabaseMaintenance))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class DatabaseMaintenance : IDatabaseMaintenance
#else
    [ApiController, Route(Endpoint)]
    [Produces("application/json")]
    [Endpoint(Name = nameof(IDatabaseMaintenance), Version = "3.0.0")]
    public class DatabaseMaintenance : Controller, IDatabaseMaintenance
#endif
    {
        public const string Endpoint = "databases";

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
        public DatabaseConfig Config { get; set; }

#endregion

#if !USE_WCF
        [HttpGet]
#endif
        public DataModel[] GetAll()
        {
            return DbContextManager.Contexts.Select(Convert).ToArray();
        }

#if !USE_WCF
        [HttpGet("model/{targetModel}")]
#endif
        public DataModel GetModel(string targetModel)
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
        public TestConnectionResponse TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new TestConnectionResponse { Result = TestConnectionResult.ConfigurationError };

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var result = targetConfigurator.TestConnection(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

#if !USE_WCF
        [HttpPost("createall")]
#endif
        public InvocationResponse CreateAll()
        {
            var bulkResult = BulkOperation(mc => mc.CreateDatabase(mc.Config), "Creation");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/create")]
#endif
        public InvocationResponse CreateDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            try
            {
                var creationResult = targetConfigurator.CreateDatabase(updatedConfig);
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
        public InvocationResponse EraseAll()
        {
            var bulkResult = BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

#if !USE_WCF
        [HttpDelete("model/{targetModel}")]
#endif
        public InvocationResponse EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            try
            {
                targetConfigurator.DeleteDatabase(updatedConfig);
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
        public InvocationResponse DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);

            var targetPath = Path.Combine(Config.SetupDataDir, targetModel);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            targetConfigurator.DumpDatabase(updatedConfig, targetPath);

            return new InvocationResponse();
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/restore")]
#endif
        public InvocationResponse RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            var filePath = Path.Combine(Config.SetupDataDir, targetModel, request.BackupFileName);
            targetConfigurator.RestoreDatabase(updatedConfig, filePath);

            return new InvocationResponse();
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/{migrationName}/migrate")]
#endif
        public DatabaseUpdateSummary MigrateDatabaseModel(string targetModel, string migrationName, DatabaseConfigModel configModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new DatabaseUpdateSummary { WasUpdated = false };

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            return targetConfigurator.MigrateDatabase(config, migrationName);
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/rollback")]
#endif
        public InvocationResponse RollbackDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var rollbackResult = targetConfigurator.RollbackDatabase(updatedConfig);

            return rollbackResult ? new InvocationResponse() : new InvocationResponse("Error while rollback!");
        }

#if !USE_WCF
        [HttpPost("model/{targetModel}/setup")]
#endif
        public InvocationResponse ExecuteSetup(string targetModel, ExecuteSetupRequest request)
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
                setupExecutor.Execute(config, targetSetup, request.Setup.SetupData);
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

        private DataModel Convert(Type contextType)
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
                AvailableMigrations = GetAvailableUpdates(dbConfig, configurator),
                AppliedMigrations = GetInstalledUpdates(dbConfig, configurator)
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
            if (!Directory.Exists(Config.SetupDataDir) || !(files = Directory.GetFiles(Config.SetupDataDir)).Any())
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
            var backupFolder = Path.Combine(Config.SetupDataDir, targetModel);

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

        private static DbMigrationsModel[] GetAvailableUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var availableMigrations = configurator.AvailableMigrations(dbConfig).ToList();
            return availableMigrations.Select(u => new DbMigrationsModel
            {
                Name = u.Name
            }).ToArray();
        }

        private static DbMigrationsModel[] GetInstalledUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var appliedMigrations = configurator.AppliedMigrations(dbConfig).ToList();
            return appliedMigrations.Select(u => new DbMigrationsModel
            {
                Name = u.Name
            }).ToArray();
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

        private string BulkOperation(Action<IModelConfigurator> operation, string operationName)
        {
            var result = string.Empty;
            foreach (var contextType in DbContextManager.Contexts)
            {
                var configurator = DbContextManager.GetConfigurator(contextType);
                try
                {
                    operation(configurator);
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
