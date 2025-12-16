// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Model;
using Moryx.Model.Configuration;
using Moryx.Runtime.Endpoints.Databases.Exceptions;
using Moryx.Runtime.Endpoints.Databases.Models;
using Moryx.Runtime.Endpoints.Databases.Request;
using Moryx.Runtime.Endpoints.Databases.Response;
using Moryx.Runtime.Endpoints.Databases.Services;
using Moryx.Tools;

namespace Moryx.Runtime.Endpoints.Databases
{
    [ApiController]
    [Route("databases")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDbContextManager _dbContextManager;
        private readonly IDatabaseConfigUpdateService _databaseUpdateService;
        private static readonly string _dataDirectory;

        static DatabaseController()
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _dataDirectory = Path.Combine(executingDirectory!, "Backups");
        }

        public DatabaseController(IDbContextManager dbContextManager)
        {
            _dbContextManager = dbContextManager;
            _databaseUpdateService = new DatabaseConfigUpdateService(_dbContextManager);
        }

        [HttpGet]
        [Authorize(Policy = RuntimePermissions.DatabaseCanView)]
        public async Task<ActionResult<DatabasesResponse>> GetAll()
        {
            var allModels = await Task.WhenAll(_dbContextManager.Contexts.Select(ConvertAsync));
            return Ok(new DatabasesResponse { Databases = allModels });
        }

        [HttpGet("{targetModel}")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanView)]
        public async Task<ActionResult<DataModel>> GetModel([FromRoute] string targetModel)
        {
            var model = _dbContextManager.Contexts.FirstOrDefault(context => TargetModelName(context) == targetModel);
            if (model == null)
                return NotFound($"Module with name \"{targetModel}\" could not be found");

            return await ConvertAsync(model);
        }

        [HttpPost("{targetModel}/config")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetAndTestConfig)]
        public async Task<ActionResult<DataModel>> SetDatabaseConfig([FromRoute] string targetModel, [FromBody] DatabaseConfigModel config)
        {
            try
            {
                var result = _databaseUpdateService.UpdateModel(targetModel, config);
                return Ok(await ConvertAsync(_dbContextManager.Contexts.First(c => TargetModelName(c) == targetModel)));
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (BadRequestException)
            {
                return BadConfigValues();
            }
        }

        private ActionResult BadConfigValues()
         => BadRequest($"Config values are not valid");

        [HttpPost("{targetModel}/config/test")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetAndTestConfig)]
        public async Task<ActionResult<TestConnectionResponse>> TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (targetConfigurator is NullModelConfigurator)
            {
                return new TestConnectionResponse { Result = TestConnectionResult.ConfigurationError };
            }

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
                return BadConfigValues();

            var result = await targetConfigurator.TestConnectionAsync(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

        [HttpPost("createall")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanCreate)]
        public async Task<ActionResult<InvocationResponse>> CreateAll()
        {
            var bulkResult = await BulkOperationAsync(mc => mc.CreateDatabaseAsync(mc.Config), "Creation");
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
                return BadConfigValues();

            try
            {
                var creationResult = await targetConfigurator.CreateDatabaseAsync(updatedConfig);
                return creationResult
                    ? new InvocationResponse()
                    : throw new Exception("Cannot create database. May be the database already exists or was misconfigured.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static DatabaseConfig UpdateConfigFromModel(DatabaseConfig dbConfig, DatabaseConfigModel configModel)
        {
            dbConfig.ConfiguratorTypename = configModel.ConfiguratorTypename;
            dbConfig.ConnectionSettings.FromDictionary(configModel.Entries);
            return dbConfig;
        }

        [HttpDelete]
        [Authorize(Policy = RuntimePermissions.DatabaseCanErase)]
        public async Task<ActionResult<InvocationResponse>> EraseAll()
        {
            var bulkResult = await BulkOperationAsync(mc => mc.DeleteDatabaseAsync(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        [HttpDelete("{targetModel}")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanErase)]
        public async Task<ActionResult<InvocationResponse>> EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
            {
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");
            }

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            if (!IsConfigValid(updatedConfig))
            {
                return BadConfigValues();
            }

            try
            {
                await targetConfigurator.DeleteDatabaseAsync(updatedConfig);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                return BadConfigValues();

            return await targetConfigurator.MigrateDatabaseAsync(config);
        }

        [HttpPost("{targetModel}/setup")]
        [Authorize(Policy = RuntimePermissions.DatabaseCanSetup)]
        public async Task<ActionResult<InvocationResponse>> ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            var contextType = _dbContextManager.Contexts.First(c => TargetModelName(c) == targetModel);
            var targetConfigurator = _dbContextManager.GetConfigurator(contextType);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            if (!IsConfigValid(config))
                return BadConfigValues();

            var setupExecutor = _dbContextManager.GetSetupExecutor(contextType);

            var targetSetup = setupExecutor.GetAllSetups().FirstOrDefault(s => s.GetType().FullName == request.Setup.Fullname);
            if (targetSetup == null)
                return NotFound("No matching setup found");

            // Provide logger for model
            // ReSharper disable once SuspiciousTypeConversion.Global
            try
            {
                await setupExecutor.ExecuteAsync(config, targetSetup, request.Setup.SetupData);
                return new InvocationResponse();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static bool IsConfigValid(DatabaseConfig config)
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

        private async Task<DataModel> ConvertAsync(Type contextType)
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
                PossibleConfigurators = GetConfigurators(contextType),
                Setups = GetAllSetups(contextType).ToArray(),
                AvailableMigrations = await GetAvailableUpdates(dbConfig, configurator),
                AppliedMigrations = await GetInstalledUpdates(dbConfig, configurator)
            };
            return model;
        }

        private static DatabaseConfigModel SerializeConfig(DatabaseConfig dbConfig)
        {
            return new()
            {
                ConfiguratorTypename = dbConfig.ConfiguratorTypename,
                Entries = dbConfig.ConnectionSettings?.ToDictionary(),
            };
        }

        private DatabaseConfigOptionModel[] GetConfigurators(Type contextType)
        {
            var configuratorTypes = _dbContextManager.GetConfigurators(contextType);

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

        private static async Task<DbMigrationsModel[]> GetAvailableUpdates(DatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            try
            {
                var availableMigrations = await configurator.AvailableMigrationsAsync(dbConfig);
                return availableMigrations.Select(migration => new DbMigrationsModel
                {
                    Name = migration
                }).ToArray();
            }
            catch (NotSupportedException)
            {
                return [];
            }
        }

        private static async Task<DbMigrationsModel[]> GetInstalledUpdates(DatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            try
            {
                var appliedMigrations = await configurator.AppliedMigrationsAsync(dbConfig);
                return appliedMigrations.Select(migration => new DbMigrationsModel
                {
                    Name = migration
                }).ToArray();
            }
            catch (NotSupportedException)
            {
                return [];
            }
        }

        private static string TargetModelName(Type contextType) => contextType.FullName;

        private async Task<string> BulkOperationAsync(Func<IModelConfigurator, Task> operation, string operationName)
        {
            var result = string.Empty;
            foreach (var contextType in _dbContextManager.Contexts)
            {
                var configurator = _dbContextManager.GetConfigurator(contextType);
                try
                {
                    await operation(configurator);
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
