// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Model.Configuration;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Databases;
using Moryx.Runtime.Modules;
using Nancy;
using Nancy.ModelBinding;

namespace Moryx.Runtime.Maintenance
{
    [Plugin(LifeCycle.Singleton, typeof(INancyModule), typeof(DatabaseMaintenance))]
    internal sealed class DatabaseMaintenance : NancyModule
    {
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

        public ModuleConfig ModuleConfig { get; set; }

        public IModuleManager ModuleManager { get; set; }

        #endregion

        public DatabaseMaintenance() : base("models")
        {
            Get("/", args => Response.AsJson(GetAll()));
            Delete("/", args => Response.AsJson(EraseAll()));

            Get("{targetModel}", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                return Response.AsJson(GetModel(targetModel));
            });

            Put("all", delegate
            {
                var response = CreateAll();
                return Response.AsJson(response);
            });

            Post("all/config", delegate
            {
                var dbConfig = this.Bind<DatabaseConfigModel>();
                SetAllConfigs(dbConfig);
                return HttpStatusCode.OK;
            });

            Put("{targetModel}", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = CreateDatabase(targetModel, dbConfig);
                return Response.AsJson(response);
            });

            Delete("{targetModel}", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = EraseDatabase(targetModel, dbConfig);
                return Response.AsJson(response);
            });

            Post("{targetModel}/config", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                SetDatabaseConfig(targetModel, dbConfig);
                return HttpStatusCode.OK;
            });

            Post("{targetModel}/config/test", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = TestDatabaseConfig(targetModel, dbConfig);
                return Response.AsJson(response);
            });

            Post("{targetModel}/dump", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = DumpDatabase(targetModel, dbConfig);
                return Response.AsJson(response);
            });

            Post("{targetModel}/restore", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var restoreModel = this.Bind<RestoreDatabaseRequest>();
                var response = RestoreDatabase(targetModel, restoreModel);
                return Response.AsJson(response);
            });

            Post("{targetModel}/migrations/{migration}", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var migration = (string) args.migration;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = MigrateDatabaseModel(targetModel, migration, dbConfig);
                return Response.AsJson(response);
            });

            Post("{targetModel}/migrations/rollback", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var dbConfig = this.Bind<DatabaseConfigModel>();
                var response = RollbackDatabase(targetModel, dbConfig);
                return Response.AsJson(response);
            });

            Post("{targetModel}/setups", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var setupReq = this.Bind<ExecuteSetupRequest>();
                var response = ExecuteSetup(targetModel, setupReq);
                return Response.AsJson(response);
            });
        }

        public DataModel[] GetAll()
        {
            return DbContextManager.Contexts.Select(Convert).ToArray();
        }

        public DataModel GetModel(string targetModel)
        {
            return Convert(DbContextManager.Contexts.FirstOrDefault(context => TargetModelName(context) == targetModel));
        }

        public void SetAllConfigs(DatabaseConfigModel config)
        {
            // Overwrite all configs
            BulkOperation(mc =>
            {
                // Save and reload config
                UpdateConfigFromModel(mc.Config, config);
                mc.UpdateConfig();
            }, "Save config");
        }

        public void SetDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return;

            // Save config and reload all DataModels
            UpdateConfigFromModel(match.Config, config);
            match.UpdateConfig();
        }

        private TestConnectionResponse TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new TestConnectionResponse { Result = TestConnectionResult.ConfigurationError };

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var result = targetConfigurator.TestConnection(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

        private InvocationResponse CreateAll()
        {
            var bulkResult = BulkOperation(mc => mc.CreateDatabase(mc.Config), "Creation");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        private InvocationResponse CreateDatabase(string targetModel, DatabaseConfigModel config)
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

        private InvocationResponse EraseAll()
        {
            var bulkResult = BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        private InvocationResponse EraseDatabase(string targetModel, DatabaseConfigModel config)
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

        private InvocationResponse DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            targetConfigurator.DumpDatabase(updatedConfig, ModuleConfig.SetupDataDir);

            return new InvocationResponse();
        }

        private InvocationResponse RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            targetConfigurator.RestoreDatabase(updatedConfig, Path.Combine(ModuleConfig.SetupDataDir, request.BackupFileName));

            return new InvocationResponse();
        }

        private DatabaseUpdateSummary MigrateDatabaseModel(string targetModel, string migrationName, DatabaseConfigModel configModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new DatabaseUpdateSummary { WasUpdated = false };

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            return targetConfigurator.MigrateDatabase(config, migrationName);
        }

        private InvocationResponse RollbackDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var rollbackResult = targetConfigurator.RollbackDatabase(updatedConfig);

            return rollbackResult ? new InvocationResponse() : new InvocationResponse("Error while rollback!");
        }

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
            if (!Directory.Exists(ModuleConfig.SetupDataDir) || !(files = Directory.GetFiles(ModuleConfig.SetupDataDir)).Any())
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

            if (!Directory.Exists(ModuleConfig.SetupDataDir))
            {
                Directory.CreateDirectory(ModuleConfig.SetupDataDir);
                return new BackupModel[0];
            }

            var allBackups = Directory.EnumerateFiles(ModuleConfig.SetupDataDir, "*.backup").Where(fileName => fileName.Contains(targetModel)).ToList();
            var backups = from backup in allBackups
                let fileName = Path.GetFileName(backup)
                let fileInfo = new FileInfo(backup)
                select new BackupModel
                {
                    FileName = fileName,
                    Size = (int)fileInfo.Length / 1024,
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
