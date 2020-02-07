// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Model;
using Moryx.Runtime.Maintenance.Contracts;
using Moryx.Runtime.Maintenance.Databases;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using Nancy;
using Nancy.ModelBinding;

namespace Moryx.Runtime.Maintenance
{
    [Plugin(LifeCycle.Singleton, typeof(INancyModule), typeof(DatabaseMaintenance))]
    internal sealed class DatabaseMaintenance : NancyModule
    {
        #region Dependencies

        [UseChild("ModelMaintenance")]
        public IModuleLogger Logger { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        public IModuleManager ModuleManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IEnumerable<IUnitOfWorkFactory> ModelFactories { get; set; }

        #endregion

        private IEnumerable<IModelConfigurator> ModelConfigurators => ModelFactories.Cast<IModelConfiguratorFactory>().Select(c => c.GetConfigurator());

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

            Post("{targetModel}/scripts", delegate (dynamic args)
            {
                var targetModel = (string)args.targetModel;
                var setupReq = this.Bind<ExecuteScriptRequest>();
                var response = ExecuteScript(targetModel, setupReq);
                return Response.AsJson(response);
            });
        }

        private DataModel[] GetAll()
        {
            return ModelConfigurators.Select(Convert).ToArray();
        }

        private DataModel GetModel(string targetModel)
        {
            return Convert(ModelConfigurators.FirstOrDefault(m => m.TargetModel == targetModel));
        }

        private void SetAllConfigs(DatabaseConfigModel config)
        {
            // Get and stop all db related modules
            var dbModules = AffectedModules();
            dbModules.ForEach(ModuleManager.StopModule);

            // Overwrite all configs
            BulkOperation(mc =>
            {
                // Save and reload config
                UpdateConfigFromModel(mc.Config, config);
                mc.UpdateConfig();
            }, "Save config");

            // Restart modules
            dbModules.ForEach(ModuleManager.StartModule);
        }

        private void SetDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return;

            // Stop all services that depend on that model
            var affectedModules = AffectedModules(targetModel);
            affectedModules.ForEach(ModuleManager.StopModule);

            // Save config and reload all DataModels
            UpdateConfigFromModel(match.Config, config);
            match.UpdateConfig();

            // Start services again
            affectedModules.ForEach(ModuleManager.StartModule);
        }

        private List<IServerModule> AffectedModules(string targetModel = null)
        {
            var affectedModules = (from module in ModuleManager.AllModules.Where(module => module.State == ServerModuleState.Running)
                                   let props = module.GetType().GetProperties()
                                   let facAtts = props.Where(prop => prop.PropertyType == typeof(IUnitOfWorkFactory))
                                                      .Select(fac => fac.GetCustomAttribute<NamedAttribute>()).ToArray()
                                   where (targetModel == null && facAtts.Any())
                                      || (targetModel != null && facAtts.Any(att => att.ComponentName == targetModel))
                                      || props.Any(prop => prop.PropertyType == typeof(IModelResolver))
                                   select module).ToList();
            return affectedModules;
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

        private InvocationResponse ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            var targetSetup = targetConfigurator.GetAllSetups().FirstOrDefault(item => item.GetType().FullName == request.Setup.Fullname);
            if (targetSetup == null)
                return new InvocationResponse("No matching setup found");

            // Provide logger for model
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (targetSetup is ILoggingComponent loggingComponent)
                loggingComponent.Logger = Logger.GetChild("Setup", loggingComponent.GetType());

            try
            {
                targetConfigurator.Execute(config, targetSetup, request.Setup.SetupData);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database setup execution failed!");
                return new InvocationResponse(ex);
            }
        }

        private InvocationResponse ExecuteScript(string targetModel, ExecuteScriptRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new InvocationResponse("No configurator found");

            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            var targetScript = targetConfigurator.GetAllScripts().FirstOrDefault(item => item.Name == request.Script.Name);
            if (targetScript == null)
                return new InvocationResponse("No matching script found");

            try
            {
                targetConfigurator.Execute(config, targetScript);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database script execution failed!");
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
            var match = ModelConfigurators.FirstOrDefault(configurator => configurator.TargetModel == model);
            return match;
        }

        private DataModel Convert(IModelConfigurator configurator)
        {
            if (configurator?.Config == null)
            {
                return null;
            }

            var dbConfig = configurator.Config;
            var model = new DataModel
            {
                TargetModel = configurator.TargetModel,
                Config = new DatabaseConfigModel
                {
                    Server = dbConfig.Host,
                    Port = dbConfig.Port,
                    Database = dbConfig.Database,
                    User = dbConfig.Username,
                    Password = dbConfig.Password
                },
                Setups = GetAllSetups(configurator),
                Backups = GetAllBackups(configurator),
                Scripts = GetAllScripts(configurator),
                AvailableMigrations = GetAvailableUpdates(dbConfig, configurator),
                AppliedMigrations = GetInstalledUpdates(dbConfig, configurator)
            };
            return model;
        }

        private SetupModel[] GetAllSetups(IModelConfigurator configurator)
        {
            var allSetups = configurator.GetAllSetups().ToList();
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

        private BackupModel[] GetAllBackups(IModelConfigurator configurator)
        {
            var targetModel = configurator.TargetModel;

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
                    Size = (int) fileInfo.Length / 1024,
                    CreationDate = fileInfo.CreationTime
                };

            return backups.ToArray();
        }

        private static ScriptModel[] GetAllScripts(IModelConfigurator configurator)
        {
            var scripts = configurator.GetAllScripts().ToList();

            return scripts.Select(s => new ScriptModel
            {
                Name = s.Name,
                IsCreationScript = s.IsCreationScript
            }).ToArray();
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

        private string BulkOperation(Action<IModelConfigurator> operation, string operationName)
        {
            var result = string.Empty;
            foreach (var configurator in ModelConfigurators)
            {
                try
                {
                    operation(configurator);
                }
                catch (Exception ex)
                {
                    Logger.LogException(LogLevel.Warning, ex, "{0} of {1} failed!", operationName, configurator.TargetModel);
                    result += $"{operationName} of {configurator.TargetModel} failed!\n";
                }
            }
            return result;
        }
    }
}
