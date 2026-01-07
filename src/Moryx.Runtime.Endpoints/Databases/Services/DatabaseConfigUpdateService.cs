// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Runtime.Endpoints.Databases.Services
{
    public class DatabaseConfigUpdateService
    {
        private readonly IDbContextManager _dbContextManager;

        public DatabaseConfigUpdateService(IDbContextManager dbContextManager)
        {
            _dbContextManager = dbContextManager;
        }

        public Type UpdateModel(Type dbContextType, DatabaseConfig config)
        {
            var configuratorType = Type.GetType(config.ConfiguratorType);

            if (string.IsNullOrEmpty(config.ConnectionString))
            {
                config.UpdateConnectionString();
            }

            if (config.ConnectionString!.Contains("<DatabaseName>"))
            {
                config.ConnectionString = config.ConnectionString.Replace("<DatabaseName>", dbContextType.Name);
                config.UpdatePropertiesFromConnectionString();
            }

            // Save config and reload all DataModels
            _dbContextManager.UpdateConfig(
                dbContextType,
                configuratorType,
                config);

            return dbContextType;
        }
    }
}

