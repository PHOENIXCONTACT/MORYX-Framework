using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Base.Serialization;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.ModuleManagement;
using Marvin.Runtime.ServerModules;
using Marvin.Serialization;
using Marvin.Threading;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    [Plugin(LifeCycle.Singleton, typeof(IModuleMaintenance))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class ModuleMaintenance : IModuleMaintenance, ILoggingComponent
    {
        #region dependency injection

        public IModuleManager ModuleManager { get; set; }
        public IParallelOperations ParallelOperations { get; set; }
        public IRuntimeConfigManager ConfigManager { get; set; }

        [UseChild("PluginMaintenance")]
        public IModuleLogger Logger { get; set; }

        #endregion

        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        public List<ServerModuleModel> GetAll()
        {
            var models = new List<ServerModuleModel>();
            foreach (var pluginService in ModuleManager.AllModules)
            {
                var model = new ServerModuleModel
                {
                    Name = pluginService.Name,
                    Assembly = ConvertAssembly(pluginService),
                    HealthState = pluginService.State.Current,
                    StartBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(pluginService).Behaviour,
                    FailureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(pluginService).Behaviour,
                    Notifications = pluginService.Notifications.Select(n => new NotificationModel(n)).ToArray()
                };

                var dependencies = ModuleManager.StartDependencies(pluginService);
                foreach (var dependency in dependencies)
                {
                    model.Dependencies.Add(new ServerModuleModel
                    {
                        Name = dependency.Name,
                        HealthState = dependency.State.Current,
                    });
                }
                models.Add(model);
            }
            return models;
        }

        public DependencyEvaluation GetDependencyEvaluation()
        {
            return new DependencyEvaluation(ModuleManager.DependencyEvaluation);
        }

        #region Service interaction
        public void Start(string moduleName)
        {
            var service = GetModule(moduleName);
            ModuleManager.StartModule(service);
        }

        public void Stop(string moduleName)
        {
            var service = GetModule(moduleName);
            ModuleManager.StopModule(service);
        }

        public void Reincarnate(string moduleName)
        {
            var service = GetModule(moduleName);
            ParallelOperations.ExecuteParallel(ModuleManager.ReincarnateModule, service);
        }

        public void ConfirmWarning(string moduleName)
        {
            var module = GetModule(moduleName);
            foreach (var notification in module.Notifications.ToArray())
            {
                notification.Confirm();
            }
            ModuleManager.InitializeModule(module);
        }
        #endregion

        #region Config
        public Config GetConfig(string moduleName)
        {
            Logger.LogEntry(LogLevel.Info, "Converting config of plugin {0}", moduleName);
            try
            {
                var module = GetModule(moduleName);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, false);
                var configModel = new Config
                {
                    Module = moduleName,
                    Entries = EntryConvert.EncodeObject(config, serialization).ToList()
                };
                return configModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to convert config of {0}", moduleName);
                return new Config { Module = moduleName };
            }
        }

        public void SetConfig(Config model, ConfigUpdateMode updateMode)
        {
            try
            {
                var module = GetModule(model.Module);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, true);
                EntryConvert.UpdateInstance(config, model.Entries, serialization);
                ConfigManager.SaveConfiguration(config, updateMode == ConfigUpdateMode.UpdateLiveAndSave);

                if (updateMode == ConfigUpdateMode.SaveAndReincarnate)
                    // This has to be done parallel so we can also reinc maintenance itself
                    ParallelOperations.ExecuteParallel(() => ModuleManager.ReincarnateModule(module));
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to save config of {0}", model.Module);
            }
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ConfigSerialization CreateSerialization(IModule module)
        {
            var host = (IContainerHost) module;
            return new ConfigSerialization(host.Container, ConfigManager);
        }

        /// <summary>
        /// Get the config type
        /// </summary>
        /// <returns></returns>
        private IConfig GetConfig(IModule module, bool copy)
        {
            var moduleType = module.GetType();
            var configType = moduleType.BaseType != null && moduleType.BaseType.IsGenericType
                ? moduleType.BaseType.GetGenericArguments()[0]
                : moduleType.Assembly.GetTypes().FirstOrDefault(type => typeof(IConfig).IsAssignableFrom(type));

            return ConfigManager.GetConfiguration(configType, copy);
        }

        public void SetStartBehaviour(string moduleName, ModuleStartBehaviour startBehaviour)
        {
            Logger.LogEntry(LogLevel.Info, "Changing start behaviour of {0} to {1}", moduleName, startBehaviour);

            var service = GetModule(moduleName);
            ModuleManager.BehaviourAccess<ModuleStartBehaviour>(service).Behaviour = startBehaviour;
        }

        public void SetFailureBehaviour(string moduleName, FailureBehaviour failureBehaviour)
        {
            Logger.LogEntry(LogLevel.Info, "Changing failure behaviour of {0} to {1}", moduleName, failureBehaviour);

            var service = GetModule(moduleName);
            ModuleManager.BehaviourAccess<FailureBehaviour>(service).Behaviour = failureBehaviour;
        }
        #endregion

        private IServerModule GetModule(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module == null)
                throw new ArgumentException("Found no module with the given name!", moduleName);
            return module;
        }

        private static AssemblyModel ConvertAssembly(IInitializable service)
        {
            var assembly = service.GetType().Assembly;
            var assemblyName = assembly.GetName();
            var bundleAtt = assembly.GetCustomAttribute<BundleAttribute>();
            var model = new AssemblyModel
            {
                Name = assemblyName.Name + ".dll",
                Version = assemblyName.Version.ToString(),
                Bundle = bundleAtt == null ? "Unknown" : bundleAtt.Bundle
            };
            return model;
        }
    }
}
