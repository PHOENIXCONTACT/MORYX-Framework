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
        public ActionResult<IEnumerable<DataModel>> GetAll()
            => Ok(_dbContextManager.Contexts.Select(Convert));


        [HttpGet("model/{targetModel}")]
        public ActionResult<DataModel> GetModel([FromRoute] string targetModel)
        {
            var model = _dbContextManager.Contexts.FirstOrDefault(context => TargetModelName(context) == targetModel);
            if (model == null)
                return NotFound($"Module with name \"{targetModel}\" could not be found");

            return Convert(model);
        }

        [HttpPost("model/{targetModel}/config")]
        public ActionResult SetDatabaseConfig([FromRoute] string targetModel, [FromBody] DatabaseConfigModel config)
        {
            var match = GetTargetConfigurator(targetModel);
            if (match == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(config))
                throw new ArgumentException("Config values are not valid", nameof(config));

            // Save config and reload all DataModels
            UpdateConfigFromModel(match.Config, config);
            match.UpdateConfig();
            return Ok();
        }

        [HttpPost("model/{targetModel}/config/test")]
        public async Task<ActionResult<TestConnectionResponse>> TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(config))
                throw new ArgumentException("Config values are not valid", nameof(config));

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
            var result = await targetConfigurator.TestConnection(updatedConfig);

            return new TestConnectionResponse { Result = result };
        }

        [HttpPost("createall")]
        public ActionResult<InvocationResponse> CreateAll()
        {
            var bulkResult = BulkOperation(mc => mc.CreateDatabase(mc.Config), "Creation");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        [HttpPost("model/{targetModel}/create")]
        public async Task<ActionResult<InvocationResponse>> CreateDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(config))
                throw new ArgumentException("Config values are not valid", nameof(config));

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
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

        [HttpDelete]
        public ActionResult<InvocationResponse> EraseAll()
        {
            var bulkResult = BulkOperation(mc => mc.DeleteDatabase(mc.Config), "Deletion");
            return string.IsNullOrEmpty(bulkResult) ? new InvocationResponse() : new InvocationResponse(bulkResult);
        }

        [HttpDelete("model/{targetModel}")]
        public ActionResult<InvocationResponse> EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(config))
                throw new ArgumentException("Config values are not valid", nameof(config));

            // Update config copy from model
            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);
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

        [HttpPost("model/{targetModel}/dump")]
        public ActionResult<InvocationResponse> DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(config))
                throw new ArgumentException("Config values are not valid", nameof(config));

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, config);

            var targetPath = Path.Combine(_dataDirectory, targetModel);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            targetConfigurator.DumpDatabase(updatedConfig, targetPath);

            return new InvocationResponse();
        }

        [HttpPost("model/{targetModel}/restore")]
        public ActionResult<InvocationResponse> RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(request.Config))
                throw new ArgumentException("Config values are not valid", nameof(request.Config));

            var updatedConfig = UpdateConfigFromModel(targetConfigurator.Config, request.Config);
            var filePath = Path.Combine(_dataDirectory, targetModel, request.BackupFileName);
            targetConfigurator.RestoreDatabase(updatedConfig, filePath);

            return new InvocationResponse();
        }

        [HttpPost("model/{targetModel}/migrate")]
        public async Task<ActionResult<DatabaseMigrationSummary>> MigrateDatabaseModel(string targetModel, DatabaseConfigModel configModel)
        {
            var targetConfigurator = GetTargetConfigurator(targetModel);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(configModel))
                throw new ArgumentException("Config values are not valid", nameof(configModel));

            var config = UpdateConfigFromModel(targetConfigurator.Config, configModel);
            return await targetConfigurator.MigrateDatabase(config);
        }

        [HttpPost("model/{targetModel}/setup")]
        public ActionResult<InvocationResponse> ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            var contextType = _dbContextManager.Contexts.First(c => TargetModelName(c) == targetModel);
            var targetConfigurator = _dbContextManager.GetConfigurator(contextType);
            if (targetConfigurator == null)
                return NotFound($"Configurator with target model \"{targetModel}\" could not be found");

            if (!IsConfigValid(request.Config))
                throw new ArgumentException("Config values are not valid", nameof(request.Config));

            // Update config copy from model
            var config = UpdateConfigFromModel(targetConfigurator.Config, request.Config);

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

        private bool IsConfigValid(DatabaseConfigModel config)
            => !string.IsNullOrEmpty(config.Database) && !string.IsNullOrEmpty(config.Server);

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

        private DataModel Convert(Type contextType)
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
                Config = new DatabaseConfigModel
                {
                    Server = dbConfig.Host,
                    Port = dbConfig.Port,
                    Database = dbConfig.Database,
                    User = dbConfig.Username,
                    Password = dbConfig.Password
                },
                Setups = GetAllSetups(contextType).ToArray(),
                Backups = GetAllBackups(contextType).ToArray(),
                AvailableMigrations = GetAvailableUpdates(dbConfig, configurator).ToArray(),
                AppliedMigrations = GetInstalledUpdates(dbConfig, configurator).ToArray()
            };
            return model;
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

        private static IEnumerable<DbMigrationsModel> GetAvailableUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var availableMigrations = configurator.AvailableMigrations(dbConfig).ToList();
            return availableMigrations.Select(u => new DbMigrationsModel
            {
                Name = u.Name
            });
        }

        private static IEnumerable<DbMigrationsModel> GetInstalledUpdates(IDatabaseConfig dbConfig, IModelConfigurator configurator)
        {
            var appliedMigrations = configurator.AppliedMigrations(dbConfig).ToList();
            return appliedMigrations.Select(u => new DbMigrationsModel
            {
                Name = u.Name
            });
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
