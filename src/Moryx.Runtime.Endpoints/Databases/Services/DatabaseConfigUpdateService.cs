// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Runtime.Endpoints.Databases.Exceptions;

namespace Moryx.Runtime.Endpoints.Databases.Services
{
    //TODO: Add internals visible to for testing
    public class DatabaseConfigUpdateService
    {
        private readonly IDbContextManager _dbContextManager;

        public DatabaseConfigUpdateService(IDbContextManager dbContextManager)
        {
            _dbContextManager = dbContextManager;
        }

        public Type UpdateModel(string targetModel, DatabaseConfig config)
        {
            var dbContextType = _dbContextManager.Contexts.First(c => c.FullName == targetModel);

            var match = _dbContextManager.GetConfigurator(dbContextType);
            if (match == null)
                throw new NotFoundException($"Configurator with target model \"{targetModel}\" could not be found");

            var configuratorType = Type.GetType(config.ConfiguratorType);

            // Save config and reload all DataModels
            _dbContextManager.UpdateConfig(
                dbContextType,
                configuratorType,
                config);

            return dbContextType;
        }
    }
}

