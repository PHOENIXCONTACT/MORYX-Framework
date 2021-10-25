// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.Model.Configuration;
using Moryx.Runtime.Maintenance.Plugins;
using Moryx.Runtime.Maintenance.Plugins.Databases;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class DatabaseMaintenanceWebClient : TestWebClientBase
    {
        public DatabaseMaintenanceWebClient(int port) : base($"http://localhost:{port}/databases/")
        {
        }

        public DataModel[] GetAll()
        {
            return Get<DataModel[]>("");
        }

        public Task<List<DataModel>> GetAllAsync()
        {
            return GetAsync<List<DataModel>>("");
        }

        public InvocationResponse EraseAll()
        {
            return DeleteAsJson<InvocationResponse>("", null);
        }

        public Task<InvocationResponse> EraseAllAsync()
        {
            return DeleteAsJsonAsync<InvocationResponse>("", null);
        }

        public DataModel GetModel(string targetModel)
        {
            return Get<DataModel>($"model/{targetModel}");
        }

        public Task<DataModel> GetModelAsync(string targetModel)
        {
            return GetAsync<DataModel>($"model/{targetModel}");
        }

        public Task SetAllConfigsAsync(DatabaseConfigModel config)
        {
            return PostAsJsonAsync("configs", config);
        }

        public void SetDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            PostAsJson($"model/{targetModel}/config", config);
        }

        public Task SetDatabaseConfigAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync($"model/{targetModel}/config", config);
        }

        public TestConnectionResponse TestDatabaseConfig(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJson<TestConnectionResponse>($"model/{targetModel}/config/test", config);
        }

        public Task<TestConnectionResponse> TestDatabaseConfigAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync<TestConnectionResponse>($"model/{targetModel}/config/test", config);
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
            return PostAsJson<InvocationResponse>($"model/{targetModel}/create", config);
        }

        public Task<InvocationResponse> CreateDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            return PostAsJsonAsync<InvocationResponse>($"model/{targetModel}/create", config);
        }

        public InvocationResponse EraseDatabase(string targetModel, DatabaseConfigModel config)
        {
            return DeleteAsJson<InvocationResponse>($"model/{targetModel}", config);
        }

        public Task<InvocationResponse> EraseDatabaseAsync(string targetModel, DatabaseConfigModel config)
        {
            return DeleteAsJsonAsync<InvocationResponse>($"model/{targetModel}", config);
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

        public DatabaseMigrationSummary MigrateDatabaseModel(string targetModel, string migrationName, DatabaseConfigModel config)
        {
            throw new System.NotImplementedException();
        }

        public Task<DatabaseMigrationSummary> MigrateDatabaseModelAsync(string targetModel, string migrationName, DatabaseConfigModel config)
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
