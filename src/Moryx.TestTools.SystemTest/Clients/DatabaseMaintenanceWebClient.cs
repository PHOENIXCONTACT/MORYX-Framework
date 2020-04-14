// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.Model;
using Moryx.Runtime.Maintenance.Plugins;
using Moryx.Runtime.Maintenance.Plugins.Databases;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class DatabaseMaintenanceWebClient : TestWebClientBase, IDatabaseMaintenance
    {
        public DatabaseMaintenanceWebClient(int port) : base($"http://localhost:{port}/DatabaseMaintenance/")
        {
        }

        public DataModel[] GetAll()
        {
            return Get<DataModel[]>("models");
        }

        public Task<List<DataModel>> GetAllAsync()
        {
            return GetAsync<List<DataModel>>("models");
        }

        public InvocationResponse EraseAll()
        {
            return DeleteAsJson<InvocationResponse>("models", null);
        }

        public Task<InvocationResponse> EraseAllAsync()
        {
            return DeleteAsJsonAsync<InvocationResponse>("models", null);
        }

        public DataModel GetModel(string targetModel)
        {
            return Get<DataModel>($"models/{targetModel}");
        }

        public Task<DataModel> GetModelAsync(string targetModel)
        {
            return GetAsync<DataModel>($"models/{targetModel}");
        }

        public void SetAllConfigs(DatabaseConfigModel config)
        {
            PostAsJson("configs", config);
        }

        public Task SetAllConfigsAsync(DatabaseConfigModel config)
        {
            return PostAsJsonAsync("configs", config);
        }

        public void SetDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            PostAsJson($"models/{targetModel}/config", config);
        }

        public Task SetDatabaseConfigAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync($"models/{targetModel}/config", config);
        }

        public TestConnectionResponse TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJson<TestConnectionResponse>($"models/{targetModel}/config/test", config);
        }

        public Task<TestConnectionResponse> TestDatabaseConfigAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync<TestConnectionResponse>($"models/{targetModel}/config/test", config);
        }

        public InvocationResponse CreateAll()
        {
            return PutAsJson<InvocationResponse>("createAll", null);
        }

        public Task<InvocationResponse> CreateAllAsync()
        {
            return PutAsJsonAsync<InvocationResponse>("createAll", null);
        }

        public InvocationResponse CreateDatabase(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJson<InvocationResponse>($"models/{targetModel}/create", config);
        }

        public Task<InvocationResponse> CreateDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync<InvocationResponse>($"models/{targetModel}/create", config);
        }

        public InvocationResponse EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            return DeleteAsJson<InvocationResponse>($"models/{targetModel}", config);
        }

        public Task<InvocationResponse> EraseDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            return DeleteAsJsonAsync<InvocationResponse>($"models/{targetModel}", config);
        }

        public InvocationResponse DumpDatabase(string targetModel, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public Task<InvocationResponse> DumpDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public InvocationResponse RestoreDatabase(string targetModel, RestoreDatabaseRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<InvocationResponse> RestoreDatabaseAsync(string targetModel, RestoreDatabaseRequest request)
        {
            throw new System.NotImplementedException();
        }

        public DatabaseUpdateSummary MigrateDatabaseModel(string targetModel, string migrationName, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public Task<DatabaseUpdateSummary> MigrateDatabaseModelAsync(string targetModel, string migrationName, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public InvocationResponse RollbackDatabase(string targetModel, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public Task<InvocationResponse> RollbackDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public InvocationResponse ExecuteSetup(string targetModel, ExecuteSetupRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<InvocationResponse> ExecuteSetupAsync(string targetModel, ExecuteSetupRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
