// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Modules;
using Moryx.ProcessData.Endpoints.Models;
using Moryx.ProcessData.Listener;
using Moryx.Runtime.Modules;

namespace Moryx.ProcessData.Endpoints.Services
{
    internal class ListenerService : IListenerService
    {
        private readonly IModuleManager _moduleManager;
        private readonly IConfigManager _configManager;

        public ListenerService(
            IModuleManager moduleManager,
            IConfigManager configManager)
        {
            _moduleManager = moduleManager;
            _configManager = configManager;
        }


        public Models.Listener GetListener(string name)
            => GetListenerConfigs()
                .FirstOrDefault(c => c.ListenerName == name)
                .ToDto();


        public ListenersResponse GetListeners()
            => new()
            {
                Listeners = GetListenerConfigs()
                    .Select(c => c.ToDto())
                    .ToList(),
            };

        public Models.Listener UpdateListener(string name, Models.Listener listenerDto)
        {
            var config = ProcessMonitorConfig();
            var listener = config.Listeners.FirstOrDefault(l => l.ListenerName == name);
            if (listener == null)
            {
                return null;
            }

            listener.MeasurandConfigs = listenerDto.Measurands.Select(m => new MeasurandConfig
            {
                IsEnabled = m.IsEnabled,
                Name = m.Name
            }).ToList();

            _moduleManager.ReincarnateModule(ProcessDataMonitor());

            return listener.ToDto();
        }


        public IServerModule GetModule(string moduleName)
            => _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);


        private IConfig GetConfig(IModule module, bool copy)
        {
            var moduleType = module.GetType();
            var configType = moduleType.BaseType != null && moduleType.BaseType.IsGenericType
                ? moduleType.BaseType.GetGenericArguments()[0]
                : moduleType.Assembly.GetTypes().FirstOrDefault(type => typeof(IConfig).IsAssignableFrom(type));

            return _configManager.GetConfiguration(configType, copy);
        }

        private List<ProcessDataListenerConfig> GetListenerConfigs()
            => FindListeners(ProcessMonitorConfig())
                .ToList();

        private Monitor.ModuleConfig ProcessMonitorConfig()
            => (Monitor.ModuleConfig)GetConfig(ProcessDataMonitor(), false);


        private IServerModule ProcessDataMonitor()
            => GetModule("ProcessDataMonitor");

        private static List<ProcessDataListenerConfig> FindListeners(IConfig config)
            => config.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(List<ProcessDataListenerConfig>))
                .SelectMany(info => (List<ProcessDataListenerConfig>)info.GetValue(config, null))
                .ToList();


        public List<string> GetAvailableBindings(Type typeInQuestion)
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => typeInQuestion.IsAssignableFrom(type) && !type.IsAbstract)
                .Select(t => t.Name)
                .Distinct()
                .ToList();
    }

    internal static class ListenerResponseMappingExtension
    {
        public static Models.Listener ToDto(this ProcessDataListenerConfig config)
            => config == null
                ? null
                : new()
                {
                    Measurands = config.MeasurandConfigs.Select(c => c.ToDto()).ToList(),
                    Name = config.ListenerName ?? config.PluginName,
                };

        private static ListenerMeasurand ToDto(this MeasurandConfig mc)
            => new()
            {
                IsEnabled = mc.IsEnabled,
                Name = mc.Name,
            };
    }
}

