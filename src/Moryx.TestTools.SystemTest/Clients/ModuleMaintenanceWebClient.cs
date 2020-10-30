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
        public ModuleMaintenanceWebClient(int port) : base($"http://localhost:{port}/modules/")
        {
        }

        public DependencyEvaluation GetDependencyEvaluation()
        {
            return Get<DependencyEvaluation>("dependencies");
        }

        public Task<DependencyEvaluation> GetDependencyEvaluationAsync()
        {
            return GetAsync<DependencyEvaluation>("dependencies");
        }

        public ServerModuleModel[] GetAll()
        {
            return Get<ServerModuleModel[]>("");
        }

        public Task<ServerModuleModel[]> GetAllAsync()
        {
            return GetAsync<ServerModuleModel[]>("");
        }

        public ServerModuleState HealthState(string moduleName)
        {
            return Get<ServerModuleState>($"module/{moduleName}/healthstate");
        }

        public Task<ServerModuleState> HealthStateAsync(string moduleName)
        {
            return GetAsync<ServerModuleState>($"module/{moduleName}/healthstate");
        }

        public NotificationModel[] Notifications(string moduleName)
        {
            return Get<NotificationModel[]>($"module/{moduleName}/notifications");
        }

        public Task<NotificationModel[]> NotificationsAsync(string moduleName)
        {
            return GetAsync<NotificationModel[]>($"module/{moduleName}/notifications");
        }

        public void Start(string moduleName)
        {
            PostAsJson($"module/{moduleName}/start", null);
        }

        public  Task StartAsync(string moduleName)
        {
            return PostAsJsonAsync($"module/{moduleName}/start", null);
        }

        public void Stop(string moduleName)
        {
            PostAsJson($"module/{moduleName}/stop", null);
        }

        public Task StopAsync(string moduleName)
        {
            return PostAsJsonAsync($"module/{moduleName}/stop", null);
        }

        public void Reincarnate(string moduleName)
        {
            PostAsJson($"module/{moduleName}/reincarnate", null);
        }

        public Task ReincarnateAsync(string moduleName)
        {
            return PostAsJsonAsync($"module/{moduleName}/reincarnate", null);
        }

        public void Update(string moduleName, ServerModuleModel module)
        {
            PostAsJson($"module/{moduleName}", module);
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
            return Get<Config>($"module/{moduleName}/config");
        }

        public Task<Config> GetConfigAsync(string moduleName)
        {
            return GetAsync<Config>($"module/{moduleName}/config");
        }

        public void SetConfig(string moduleName, SaveConfigRequest request)
        {
            PostAsJson($"module/{moduleName}/config", request);
        }

        public Task SetConfigAsync(string moduleName, SaveConfigRequest request)
        {
            return PostAsJsonAsync($"module/{moduleName}/config", request);
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
