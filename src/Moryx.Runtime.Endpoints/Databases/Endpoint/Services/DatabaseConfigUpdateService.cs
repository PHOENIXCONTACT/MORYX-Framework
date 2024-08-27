using Moryx.Model;
using Moryx.Model.Configuration;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions;
using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var dbConfig = (IDatabaseConfig)Activator.CreateInstance(configType);
            var updatedConfig = UpdateConfigFromModel(dbConfig, config);
            if (!updatedConfig.IsValid())
                requestErrors.Add("Requested config values aren't valid");

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

        private static IDatabaseConfig UpdateConfigFromModel(IDatabaseConfig dbConfig, DatabaseConfigModel configModel)
        {
            //dbConfig.ConfiguratorTypename = configModel.ConfiguratorTypename;
            dbConfig.ConnectionSettings.FromDictionary(configModel.Entries);
            return dbConfig;
        }
    }
}
