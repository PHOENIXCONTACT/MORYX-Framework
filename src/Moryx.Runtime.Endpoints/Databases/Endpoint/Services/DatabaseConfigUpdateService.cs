// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Configuration;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Services
{
    public class DatabaseConfigUpdateService : IDatabaseConfigUpdateService
    {
        private readonly IDbContextManager _dbContextManager;

        public DatabaseConfigUpdateService(IDbContextManager dbContextManager)
        {
            _dbContextManager = dbContextManager;
        }

        public Type UpdateModel(string targetModel, DatabaseConfigModel config)
        {
            var requestErrors = new List<string>();
            var dbContextType = FindContext(targetModel);

            var match = _dbContextManager.GetConfigurator(dbContextType);
            if (match == null)
                throw new NotFoundException($"Configurator with target model \"{targetModel}\" could not be found");

            var configuratorType = Type.GetType(config.ConfiguratorTypename);

            // Assert config
            var configType = configuratorType.BaseType.GenericTypeArguments.First();
            var dbConfig = (DatabaseConfig)Activator.CreateInstance(configType);
            var updatedConfig = UpdateConfigFromModel(dbConfig, config);
            if (!updatedConfig.IsValid())
                requestErrors.Add("Requested config values aren't valid");

            // If database was not set, use context name as database name
            if (string.IsNullOrEmpty(updatedConfig.ConnectionSettings.Database))
            {
                updatedConfig.ConnectionSettings.Database = dbContextType.Name;
            }

            // Save config and reload all DataModels
            _dbContextManager.UpdateConfig(
                dbContextType,
                configuratorType,
                dbConfig);

            if (requestErrors.Any())
                throw new BadRequestException(requestErrors.ToArray());

            return FindContext(targetModel);
        }

        private Type FindContext(string targetModel)
        {
            return _dbContextManager.Contexts.First(c => c.FullName == targetModel);
        }

        private static DatabaseConfig UpdateConfigFromModel(DatabaseConfig dbConfig, DatabaseConfigModel configModel)
        {
            //dbConfig.ConfiguratorTypename = configModel.ConfiguratorTypename;
            dbConfig.ConnectionSettings.FromDictionary(configModel.Entries);
            return dbConfig;
        }
    }
}

