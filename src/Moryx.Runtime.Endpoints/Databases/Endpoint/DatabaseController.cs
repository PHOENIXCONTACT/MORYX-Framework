// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moryx.Model;
using Moryx.Model.Configuration;
using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Response;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Request;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Moryx.Tools;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint
{
    [ApiController]
    [Route("databases")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDbContextManager _dbContextManager;
        private readonly string _dataDirectory = @".\Backups\";

        public DatabaseController(IDbContextManager dbContextManager)
        {
            _dbContextManager = dbContextManager;
        }

        [HttpGet]
        [Authorize(Policy = RuntimePermissions.DatabaseCanView)]
        public async Task<ActionResult<ResponseModel<ListOfDataModels>>> GetAll()
            => OkResponse(new ListOfDataModels { Databases = await Task.WhenAll(_dbContextManager.Contexts.Select(Convert)) });


        [HttpGet("{targetModel}")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanView)]
        public async Task<ActionResult<ResponseModel<DataModel>>> GetModel([FromRoute] string targetModel)
        {
            var model = _dbContextManager.Contexts.FirstOrDefault(context => TargetModelName(context) == targetModel);
            if (model == null)
                return NotFound($"Module with name \"{targetModel}\" could not be found");

            var result = await Convert(model);
            return OkResponse(result);
        }

        [HttpPost("{targetModel}/config")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetAndTestConfig)]
        public async Task<ActionResult<ResponseModel<DataModel>>> SetDatabaseConfig([FromRoute] string targetModel, [FromBody] DatabaseConfigModel config)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var dbContextType = _dbContextManager.Contexts.First(c => TargetModelName(c) == targetModel);
            var configuratorType = Type.GetType(config.ConfiguratorTypename);

            // Assert config
            var configType = configuratorType.BaseType.GenericTypeArguments.First();
            var dbConfig = (IDatabaseConfig)Activator.CreateInstance(configType);
            var updatedConfig = UpdateConfigFromModel(dbConfig, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigResponse<DataModel>(config);

            // Save config and reload all DataModels
            _dbContextManager.UpdateConfig(
                dbContextType,
                configuratorType,
                dbConfig);
            return OkResponse(await Convert(dbContextType));
        }

        private ActionResult<ResponseModel<T>> OkResponse<T>(T result)
            => Ok(new ResponseModel<T>() { Result = result });

        private static ResponseModel<T> ErrorResponse<T>(string error)
            => new() { Errors = new[] { error } };

        private ActionResult<ResponseModel<T>> BadConfigResponse<T>(DatabaseConfigModel subject)
        {
            return BadRequest(ErrorResponse<T>($"Config values are not valid"));
        }

        private ActionResult BadConfigValues(DatabaseConfigModel subject)
        {
            return BadRequest($"Config values are not valid");
        }

        [HttpPost("{targetModel}/config/test")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetAndTestConfig)]
        public async Task<ActionResult<TestConnectionResponse>> TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues(config);

            var result = await targetConfigurator.TestConnection(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

        [HttpPost("createall")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanCreate)]
        public ActionResult<InvocationResponse> CreateAll()
        {
            var bulkResult = BulkOperation(mc => mc.CreateDatabase(mc.Config), "Creation");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        [HttpPost("{targetModel}/create")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanCreate)]
        public async Task<ActionResult<InvocationResponse>> CreateDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues(config);

            try
            {
                var creationResult = await targetConfigurator.CreateDatabase(updatedConfig);
                return creationResult
                    ? new InvocationResponse()
                    : throw new Exception("Cannot create database. May be the database already exists or was misconfigured.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private IDatabaseConfig UpdateConfigFromModel(IDatabaseConfig dbConfig, DatabaseConfigModel configModel)
        {
            dbConfig.ConnectionSettings.FromDictionary(configModel.Entries);
            return dbConfig;
        }

        [HttpDelete]
        [Authorize(Policy = RuntimePermissions.DatabaseCanErase)]
        public ActionResult<InvocationResponse> EraseAll()
        {
            var bulkResult = BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        [HttpDelete("{targetModel}")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanErase)]
        public ActionResult<InvocationResponse> EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues(config);

            try
            {
                targetConfigurator.DeleteDatabase(updatedConfig);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("{targetModel}/dump")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanDumpAndRestore)]
        public ActionResult<InvocationResponse> DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues(config);


            var targetPath = Path.Combine(_dataDirectory, targetModel);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            targetConfigurator.DumpDatabase(updatedConfig, targetPath);

            return new InvocationResponse();
        }

        [HttpPost("{targetModel}/restore")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanDumpAndRestore)]
        public ActionResult<InvocationResponse> RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues(request.Config);

            var filePath = Path.Combine(_dataDirectory, targetModel, request.BackupFileName);
            targetConfigurator.RestoreDatabase(updatedConfig, filePath);

            return new InvocationResponse();
        }

        [HttpPost("{targetModel}/migrate")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanMigrateModel)]
        public async Task<ActionResult<DatabaseMigrationSummary>> MigrateDatabaseModel(string targetModel, DatabaseConfigModel configModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            if (!IsConfigValid(config))
                return BadConfigValues(configModel);

            return await targetConfigurator.MigrateDatabase(config);
        }

        [HttpPost("{targetModel}/setup")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetup)]
        public ActionResult<InvocationResponse> ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            var contextType = _dbContextManager.Contexts.First(c => TargetModelName(c) == targetModel);
            var targetConfigurator = _dbContextManager.GetConfigurator(contextType);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            if (!IsConfigValid(config))
                return BadConfigValues(request.Config);


            var setupExecutor = _dbContextManager.GetSetupExecutor(contextType);

            var targetSetup = setupExecutor.GetAllSetups().FirstOrDefault(s => s.GetType().FullName == request.Setup.Fullname);
            if (targetSetup == null)
                return NotFound("No matching setup found");

            // Provide logger for model
            // ReSharper disable once SuspiciousTypeConversion.Global
            try
            {
                setupExecutor.Execute(config, targetSetup, request.Setup.SetupData);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static bool IsConfigValid(IDatabaseConfig config)
        {
            return config.IsValid();
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
            var context = _dbContextManager.Contexts.First(c => TargetModelName(c) == model);
            return _dbContextManager.GetConfigurator(context);
        }

        private async Task<DataModel> Convert(Type contextType)
        {
            var configurator = _dbContextManager.GetConfigurator(contextType);
            if (configurator?.Config == null)
            {
                return null;
            }

            var dbConfig = configurator.Config;
            var model = new DataModel
            {
                TargetModel = TargetModelName(contextType),
                Config = SerializeConfig(dbConfig),
                PossibleConfigurators = GetConfigurators(),
                Setups = GetAllSetups(contextType).ToArray(),
                Backups = GetAllBackups(contextType).ToArray(),
                AvailableMigrations = await GetAvailableUpdates(dbConfig, configurator),
                AppliedMigrations = await GetInstalledUpdates(dbConfig, configurator)
            };
            return model;
        }

        private static DatabaseConfigModel SerializeConfig(IDatabaseConfig dbConfig)
        {
            return new()
            {
                ConfiguratorTypename = dbConfig.ConfiguratorTypename,
                Entries = dbConfig.ConnectionSettings?.ToDictionary(),
            };
        }

        private static DatabaseConfigOptionModel[] GetConfigurators()
        {
            var configuratorTypes = ReflectionTool
                .GetPublicClasses(typeof(IModelConfigurator), delegate (Type type)
                {
                    return type != typeof(NullModelConfigurator);
                }).ToList();

            var result = configuratorTypes
                .Select(type => new DatabaseConfigOptionModel
                {
                    Name = type.GetDisplayName() ?? type.Name,
                    ConfiguratorTypename = type.AssemblyQualifiedName,
                    Properties = GetConfiguratorProperties(type),
                }).ToArray();
            return result;
        }

        private static DatabaseConfigOptionPropertyModel[] GetConfiguratorProperties(Type type)
        {
            var configType = type.BaseType.GenericTypeArguments.First()
                .BaseType.GenericTypeArguments.First();

            var properties = configType.GetProperties();
            return properties
                .Where(p => p.GetCustomAttribute<DataMemberAttribute>() != null)
                .Select(p => new DatabaseConfigOptionPropertyModel
                {
                    Name = p.Name,
                    Default = p.GetCustomAttribute<DefaultValueAttribute>()?.Value.ToString(),
                    Required = p.GetCustomAttribute<RequiredAttribute>() != null,
                }).ToArray();
        }

        private IEnumerable<SetupModel> GetAllSetups(Type contextType)
        {
            var setupExecutor = _dbContextManager.GetSetupExecutor(contextType);
            var allSetups = setupExecutor.GetAllSetups();
            var setups = allSetups.Where(setup => string.IsNullOrEmpty(setup.SupportedFileRegex))
                                  .Select(ConvertSetup).OrderBy(setup => setup.SortOrder).ToList();
            string[] files;
            if (!Directory.Exists(_dataDirectory) || !(files = Directory.GetFiles(_dataDirectory)).Any())
                return setups.ToArray();

            var fileSetups = allSetups.Where(setup => !string.IsNullOrEmpty(setup.SupportedFileRegex))
                                      .Select(ConvertSetup).ToList();
            foreach (var setup in fileSetups)
            {
                var regex = new Regex(setup.SupportedFileRegex);
                var matchingFiles = files.Where(file => regex.IsMatch(Path.GetFileName(file)));
                setups.AddRange(matchingFiles.Select(setup.CopyWithFile));
            }
            return setups.OrderBy(setup => setup.SortOrder);
        }

        private IEnumerable<BackupModel> GetAllBackups(Type contextType)
        {
            var targetModel = TargetModelName(contextType);
            var backupFolder = Path.Combine(_dataDirectory, targetModel);

            if (!Directory.Exists(backupFolder))
                return Array.Empty<BackupModel>();

            var allBackups = Directory.EnumerateFiles(backupFolder, "*.backup").ToList();
            var backups = from backup in allBackups
                          let fileName = Path.GetFileName(backup)
                          let fileInfo = new FileInfo(backup)
                          select new BackupModel
                          {
                              FileName = fileName,
                              Size = (int)fileInfo.Length / 1024,
                              CreationDate = fileInfo.CreationTime
                          };

            return backups;
        }

        private static async Task<DbMigrationsModel[]> GetAvailableUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            try
            {
                var availableMigrations = await configurator.AvailableMigrations(dbConfig);
                return availableMigrations.Select(migration => new DbMigrationsModel
                {
                    Name = migration
                }).ToArray();
            }
            catch (NotSupportedException)
            {
                return Array.Empty<DbMigrationsModel>();
            }
        }

        private static async Task<DbMigrationsModel[]> GetInstalledUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            try
            {
                var appliedMigrations = await configurator.AppliedMigrations(dbConfig);
                return appliedMigrations.Select(migration => new DbMigrationsModel
                {
                    Name = migration
                }).ToArray();
            }
            catch (NotSupportedException)
            {
                return Array.Empty<DbMigrationsModel>();
            }
        }

        private static string TargetModelName(Type contextType) => contextType.FullName;

        private string BulkOperation(Action<IModelConfigurator> operation, string operationName)
        {
            var result = string.Empty;
            foreach (var contextType in _dbContextManager.Contexts)
            {
                var configurator = _dbContextManager.GetConfigurator(contextType);
                try
                {
                    operation(configurator);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{operationName} of {TargetModelName(contextType)} failed!\n", ex);
                }
            }
            return result;
        }
    }
}
