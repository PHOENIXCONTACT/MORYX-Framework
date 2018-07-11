using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Configuration;
using Marvin.Runtime.Container;
using Marvin.Runtime.Modules;
using Marvin.Serialization;
using Marvin.Threading;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    [Plugin(LifeCycle.Singleton, typeof(IModuleMaintenance))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class ModuleMaintenance : IModuleMaintenance, ILoggingComponent
    {
        #region dependency injection

        public IModuleManager ModuleManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IRuntimeConfigManager ConfigManager { get; set; }

        [UseChild("ModuleMaintenance")]
        public IModuleLogger Logger { get; set; }

        #endregion

        public ServerModuleModel[] GetAll()
        {
            var models = new List<ServerModuleModel>(ModuleManager.AllModules.Count());
            foreach (var module in ModuleManager.AllModules)
            {
                var model = new ServerModuleModel
                {
                    Name = module.Name,
                    Assembly = ConvertAssembly(module),
                    HealthState = module.State,
                    StartBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(module).Behaviour,
                    FailureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(module).Behaviour,
                    Notifications = module.Notifications.Select(n => new NotificationModel(n)).ToArray()
                };

                var dependencies = ModuleManager.StartDependencies(module);
                foreach (var dependency in dependencies)
                {
                    model.Dependencies.Add(new ServerModuleModel
                    {
                        Name = dependency.Name,
                        HealthState = dependency.State
                    });
                }
                models.Add(model);
            }
            return models.ToArray();
        }

        public ServerModuleState HealthState(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return module.State;
            }

            var ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            return ServerModuleState.Failure;
        }

        public NotificationModel[] Notifications(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return module.Notifications.Select(n => new NotificationModel(n)).ToArray();
            }

            var ctx = WebOperationContext.Current;
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            return new NotificationModel[0];
        }

        public DependencyEvaluation GetDependencyEvaluation()
        {
            return new DependencyEvaluation(ModuleManager.DependencyEvaluation);
        }

        public void Start(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StartModule(module);
        }

        public void Stop(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StopModule(module);
        }

        public void Reincarnate(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ParallelOperations.ExecuteParallel(ModuleManager.ReincarnateModule, module);
        }

        public void Update(string moduleName, ServerModuleModel module)
        {
            var serverModule = GetModuleFromManager(moduleName);

            var startBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(serverModule);
            if (startBehaviour.Behaviour != module.StartBehaviour)
            {
                Logger.LogEntry(LogLevel.Info, "Changing start behaviour of {0} to {1}", moduleName, module.StartBehaviour);
                startBehaviour.Behaviour = module.StartBehaviour;
            }

            var failureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(serverModule);
            if (failureBehaviour.Behaviour != module.FailureBehaviour)
            {
                Logger.LogEntry(LogLevel.Info, "Changing failure behaviour of {0} to {1}", moduleName, module.FailureBehaviour);
                failureBehaviour.Behaviour = module.FailureBehaviour;
            }
        }

        public void ConfirmWarning(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            foreach (var notification in module.Notifications.ToArray())
            {
                notification.Confirm();
            }
            ModuleManager.InitializeModule(module);
        }

        public Config GetConfig(string moduleName)
        {
            Logger.LogEntry(LogLevel.Info, "Converting config of plugin {0}", moduleName);
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, false);
                var configModel = new Config
                {
                    Entries = EntryConvert.EncodeObject(config, serialization).ToList()
                };
                return configModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to convert config of {0}", moduleName);
                HttpHelper.SetStatusCode(HttpStatusCode.InternalServerError);
                return null;
            }
        }

        public void SetConfig(string moduleName, SaveConfigRequest request)
        {
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, true);
                EntryConvert.UpdateInstance(config, request.Config.Entries, serialization);
                ConfigManager.SaveConfiguration(config, request.UpdateMode == ConfigUpdateMode.UpdateLiveAndSave);

                if (request.UpdateMode == ConfigUpdateMode.SaveAndReincarnate)
                    // This has to be done parallel so we can also reinc maintenance itself
                    ParallelOperations.ExecuteParallel(() => ModuleManager.ReincarnateModule(module));
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to save config of {0}", moduleName);
                HttpHelper.SetStatusCode(HttpStatusCode.InternalServerError);
            }
        }

        public MethodEntry[] GetMethods(string moduleName)
        {
            var methods = new MethodEntry[] {};
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule?.Console != null)
            {
                methods = EntryConvert.EncodeMethods(serverModule.Console, CreateEditorVisibleSerialization(serverModule)).ToArray();
            }

            return methods;
        }

        public Entry InvokeMethod(string moduleName, MethodEntry method)
        {
            Entry result = null;
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule != null && method != null)
            {
                try
                {
                    result = EntryConvert.InvokeMethod(serverModule.Console, method,
                        CreateEditorVisibleSerialization(serverModule));
                }
                catch (Exception e)
                {
                    result = new Entry
                    {
                        Description = $"Error while invoking function: {method.DisplayName}",
                        Key = new EntryKey {Identifier = "0", Name = "Error description"},
                        Value = new EntryValue {Current = e.Message, Type = EntryValueType.String}
                    };
                }
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateSerialization(IModule module)
        {
            var host = (IContainerHost) module;
            return new PossibleValuesSerialization(host.Container, ConfigManager);
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateEditorVisibleSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            return new AdvancedEditorVisibleSerialization(host.Container, ConfigManager);
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

        private IServerModule GetModuleFromManager(string moduleName)
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