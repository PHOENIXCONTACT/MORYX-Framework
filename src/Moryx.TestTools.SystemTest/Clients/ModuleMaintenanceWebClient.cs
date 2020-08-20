// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Moryx.Runtime.Maintenance.Plugins.Modules;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class ModuleMaintenanceWebClient : TestWebClientBase
    {
        public ModuleMaintenanceWebClient(int port) : base($"http://localhost:{port}/ModuleMaintenance/")
        {
        }

        public DependencyEvaluation GetDependencyEvaluation()
        {
            return Get<DependencyEvaluation>("dependencyEvaluation");
        }

        public Task<DependencyEvaluation> GetDependencyEvaluationAsync()
        {
            return GetAsync<DependencyEvaluation>("dependencyEvaluation");
        }

        public ServerModuleModel[] GetAll()
        {
            return Get<ServerModuleModel[]>("modules");
        }

        public Task<ServerModuleModel[]> GetAllAsync()
        {
            return GetAsync<ServerModuleModel[]>("modules");
        }

        public ServerModuleState HealthState(string moduleName)
        {
            return Get<ServerModuleState>($"modules/{moduleName}/healthstate");
        }

        public Task<ServerModuleState> HealthStateAsync(string moduleName)
        {
            return GetAsync<ServerModuleState>($"modules/{moduleName}/healthstate");
        }

        public NotificationModel[] Notifications(string moduleName)
        {
            return Get<NotificationModel[]>($"modules/{moduleName}/notifications");
        }

        public Task<NotificationModel[]> NotificationsAsync(string moduleName)
        {
            return GetAsync<NotificationModel[]>($"modules/{moduleName}/notifications");
        }

        public void Start(string moduleName)
        {
            PostAsJson($"modules/{moduleName}/start", null);
        }

        public  Task StartAsync(string moduleName)
        {
            return PostAsJsonAsync($"modules/{moduleName}/start", null);
        }

        public void Stop(string moduleName)
        {
            PostAsJson($"modules/{moduleName}/stop", null);
        }

        public Task StopAsync(string moduleName)
        {
            return PostAsJsonAsync($"modules/{moduleName}/stop", null);
        }

        public void Reincarnate(string moduleName)
        {
            PostAsJson($"modules/{moduleName}/reincarnate", null);
        }

        public Task ReincarnateAsync(string moduleName)
        {
            return PostAsJsonAsync($"modules/{moduleName}/reincarnate", null);
        }

        public void Update(string moduleName, ServerModuleModel module)
        {
            PostAsJson($"modules/{moduleName}", module);
        }

        public Task UpdateAsync(string moduleName, ServerModuleModel module)
        {
            return PostAsJsonAsync($"modules/{moduleName}", module);
        }

        public void ConfirmWarning(string moduleName)
        {
            throw new NotImplementedException();
        }

        public Task ConfirmWarningAsync(string moduleName)
        {
            throw new NotImplementedException();
        }

        public Config GetConfig(string moduleName)
        {
            return Get<Config>($"modules/{moduleName}/config");
        }

        public Task<Config> GetConfigAsync(string moduleName)
        {
            return GetAsync<Config>($"modules/{moduleName}/config");
        }

        public void SetConfig(string moduleName, SaveConfigRequest request)
        {
            PostAsJson($"modules/{moduleName}/config", request);
        }

        public Task SetConfigAsync(string moduleName, SaveConfigRequest request)
        {
            return PostAsJsonAsync($"modules/{moduleName}/config", request);
        }

        public MethodEntry[] GetMethods(string moduleName)
        {
            throw new NotImplementedException();
        }

        public Task<MethodEntry[]> GetMethodsAsync(string moduleName)
        {
            throw new NotImplementedException();
        }

        public Entry InvokeMethod(string moduleName, MethodEntry method)
        {
            throw new NotImplementedException();
        }

        public Task<Entry> InvokeMethodAsync(string moduleName, MethodEntry method)
        {
            throw new NotImplementedException();
        }
    }
}
