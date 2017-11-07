using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Model;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Maintenance.Filesystem;
using Marvin.Runtime.ModuleManagement;
using Marvin.Threading;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    [Plugin(LifeCycle.Singleton, typeof(IDatabaseMaintenance))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class DatabaseMaintenance : IDatabaseMaintenance
    {
        // Castle dependency injection
        public IRuntimeConfigManager ConfigManager { get; set; }
        public IModuleManager ModuleManager { get; set; }
        public IParallelOperations ParallelOperations { get; set; }
        public IEnumerable<IModelConfigurator> ModelConfigurators { get; set; }
        public FileSystemPathProvider PathProvider { get; set; }

        /// <summary>
        /// Logger of this component
        /// </summary>
        [UseChild("ModelMaintenance")]
        public IModuleLogger Logger { get; set; }

        public DatabaseConfig Config { get; set; }

        public DataModel[] GetDataModels()
        {
            return ModelConfigurators.Select(Convert).ToArray();
        }

        public void SetAllConfigs(DatabaseConfigModel config)
        {
            // Get and stop all db related modules
            var dbModules = AffectedModules();
            dbModules.ForEach(ModuleManager.StopModule);

            // Overwrite all configs
            BulkOperation(mc =>
            {
                // Save and reload config
                var conf = UpdateConfigFromModel(mc.Config, config);
                ConfigManager.SaveConfiguration(conf);
                mc.ResponsibleFactory.ReloadConfiguration();
            }, "Save config");

            // Restart modules
            dbModules.ForEach(ModuleManager.StartModule);
        }

        public void SetDatabaseConfig(string targetModel, DatabaseConfigModel model)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return;

            // Stop all services that depend on that model
            var affectedModules = AffectedModules(targetModel);
            affectedModules.ForEach(ModuleManager.StopModule);

            // Save config and reload all DataModels
            var conf = UpdateConfigFromModel(match.Config, model);
            ConfigManager.SaveConfiguration(conf);
            match.ResponsibleFactory.ReloadConfiguration();

            // Start services again
            affectedModules.ForEach(ModuleManager.StartModule);
        }

        private List<IServerModule> AffectedModules(string targetModel = null)
        {
            var affectedModules = (from module in ModuleManager.AllModules.Where(module => module.State.Current == ServerModuleState.Running)
                                   let props = module.GetType().GetProperties()
                                   let facAtts = props.Where(prop => prop.PropertyType == typeof(IUnitOfWorkFactory))
                                                      .Select(fac => fac.GetCustomAttribute<NamedAttribute>()).ToArray()
                                   where (targetModel == null && facAtts.Any())
                                      || (targetModel != null && facAtts.Any(att => att.ComponentName == targetModel))
                                      || props.Any(prop => prop.PropertyType == typeof(IModelResolver))
                                   select module).ToList();
            return affectedModules;
        }

        public bool TestDatabaseConfig(string targetModel, DatabaseConfigModel model)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return false;

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, model);
            bool result = targetConfigurator.TestConnection(config);

            return result;
        }

        public string CreateAll()
        {
            return BulkOperation(mc => mc.CreateDatabase(mc.Config),"Creation");
        }

        public string CreateDatabase(string targetModel, DatabaseConfigModel model)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return "No configurator found";

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, model);
            try
            {
                targetConfigurator.CreateDatabase(config);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database creation failed!");
                return string.Format("Database creation failed with exception: {0}\n Inner exception: {1}",
                                    ex.Message, ex.InnerException == null ? "none" : ex.InnerException.Message);
            }
        }

        public string EraseAll()
        {
            return BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
        }

        public string EraseDatabase(string targetModel, DatabaseConfigModel model)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return "No configurator found";

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, model);
            try
            {
                targetConfigurator.DeleteDatabase(config);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database deletion failed!");
                return string.Format("Deleting data base failed with exception: {0}\n Inner exception: {1}",
                                    ex.Message, ex.InnerException == null ? "none" : ex.InnerException.Message);
            }
        }

        public DumpResult DumpDatabase(string targetModel, DatabaseConfigModel model)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return new DumpResult("No configurator found");

            var config = UpdateConfigFromModel(targetConfigurator.Config, model);
            var dumpName = string.Format("{0}_{1}_{2}.sql", targetModel, model.Database, DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss"));
            ParallelOperations.ExecuteParallel(context => context.Configurator.DumpDatabase(config, string.Format(@".\Backups\{0}", context.DumpName)), 
                                               new { Configurator = targetConfigurator, DumpName = dumpName});
            return new DumpResult(string.Empty) { DumpName = dumpName };
        }

        public string RestoreDatabase(string targetModel, DatabaseConfigModel configModel, BackupModel backupModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return "No configurator found";

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            ParallelOperations.ExecuteParallel(context => context.Configurator.RestoreDatabase(config, string.Format(@".\Backups\{0}", context.FileName)),
                                               new { Configurator = targetConfigurator, backupModel.FileName });
            return string.Empty;
        }

        public string ExecuteSetup(string targetModel, DatabaseConfigModel model, SetupModel setup)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return "No configurator found";

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, model);
            var targetSetup = targetConfigurator.GetAllSetups().FirstOrDefault(item => item.GetType().FullName == setup.Fullname);
            if (targetSetup == null)
                return "No matching setup found!";
            
            // Provide logger for model
            if (targetSetup is ILoggingComponent)
                ((ILoggingComponent) targetSetup).Logger = Logger.GetChild("Setup", targetSetup.GetType());
            try
            {
                targetConfigurator.Execute(config, targetSetup, setup.SetupData);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Database setup execution failed!");
                return string.Format("Setup execution failed with exception: {0}\n Inner exception: {1}",
                                    ex.Message, ex.InnerException == null ? "none" : ex.InnerException.Message);
            }
        }

        public string ExecuteScript(string targetModel, DatabaseConfigModel model, ScriptModel script)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return "No configurator found";

            return "THOMAS IMPLEMENT THIS FUNCTION!!!";
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
            var dbConfig = configurator.Config;
            var model = new DataModel
            {
                TargetModel = configurator.TargetModel,
                Config = new DatabaseConfigModel
                {
                    Server = dbConfig.Server,
                    Port = dbConfig.Port,
                    Database = dbConfig.Database,
                    Schema = dbConfig.Schema,
                    User = dbConfig.User,
                    Password = dbConfig.Password
                },
                Setups = GetAllSetups(configurator),
                Backups = GetAllBackups(configurator),
                Scripts = GetAllScripts(configurator)
            };
            return model;
        }

        private SetupModel[] GetAllSetups(IModelConfigurator configurator)
        {
            var allSetups = configurator.GetAllSetups().ToList();
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

        private BackupModel[] GetAllBackups(IModelConfigurator configurator)
        {
            var backupModels = new List<BackupModel>();

            var tagetModel = configurator.TargetModel;
            var backupDir = @".\Backups\";

            if (!Directory.Exists(backupDir))
                return new BackupModel[0];

            var allBackups = Directory.GetFiles(backupDir, "*.sql").ToList();

            foreach (var backup in allBackups)
            {
                var fileName = Path.GetFileName(backup);
                var isForTagetModel = fileName.StartsWith(tagetModel);
                var fileInfo = new FileInfo(backup);
                var backupModel = new BackupModel()
                {
                    FileName = fileName,
                    Size = (int)fileInfo.Length / 1024,
                    IsForTargetModel = isForTagetModel,
                    CreationDate = fileInfo.CreationTime
                };
                backupModels.Add(backupModel);
            }

            return backupModels.ToArray();
        }

        private ScriptModel[] GetAllScripts(IModelConfigurator configurator)
        {
            var scripts = configurator.GetAllScripts().ToList();

            return scripts.Select(s => new ScriptModel
            {
                Name = s.Name,
                IsCreationScript = s.IsCreationScript
            }).ToArray();
        }

        private IDatabaseConfig UpdateConfigFromModel(IDatabaseConfig dbConfig, DatabaseConfigModel model)
        {
            dbConfig.Server = model.Server;
            dbConfig.Port = model.Port;
            dbConfig.Database = model.Database;
            dbConfig.Schema = model.Schema;
            dbConfig.User = model.User;
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
                    result += string.Format("{0} of {1} failed!\n", operationName, configurator.TargetModel);
                }
            }
            return result;
        }
    }
}
