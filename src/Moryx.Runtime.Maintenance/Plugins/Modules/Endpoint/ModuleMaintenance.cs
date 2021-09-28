// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Threading;

#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
#if USE_WCF
    [Plugin(LifeCycle.Singleton, typeof(IModuleMaintenance))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    public class ModuleMaintenance : IModuleMaintenance, ILoggingComponent
#else
    [ApiController, Route(Endpoint)]
    [Produces("application/json")]
    [Endpoint(Name = nameof(IModuleMaintenance), Version = "3.0.0")]
    public class ModuleMaintenance : Controller, IModuleMaintenance, ILoggingComponent
#endif
    {
        internal const string Endpoint = "modules";

        #region Dependencies

        public IModuleManager ModuleManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IRuntimeConfigManager ConfigManager { get; set; }

        [UseChild("ModuleMaintenance")]
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("dependencies")]
#endif
        public DependencyEvaluation GetDependencyEvaluation()
        {
            return new DependencyEvaluation(ModuleManager.DependencyTree);
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet]
#endif
        public ServerModuleModel[] GetAll()
        {
            var models = new List<ServerModuleModel>(ModuleManager.AllModules.Count());
            foreach (var module in ModuleManager.AllModules)
            {
                var notifications = module.Notifications.ToArray();

                var model = new ServerModuleModel
                {
                    Name = module.Name,
                    Assembly = ConvertAssembly(module),
                    HealthState = module.State,
                    StartBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(module).Behaviour,
                    FailureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(module).Behaviour,
                    Notifications = notifications.Select(n => new NotificationModel(n)).ToArray()
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

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("module/{moduleName}/healthstate")]
#endif
        public ServerModuleState HealthState(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return module.State;
            }

#if USE_WCF
            var ctx = WebOperationContext.Current;
            // ReSharper disable once PossibleNullReferenceException
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
            Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif

            return ServerModuleState.Failure;
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("module/{moduleName}/notifications")]
#endif
        public NotificationModel[] Notifications(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return module.Notifications.Select(n => new NotificationModel(n)).ToArray();
            }

#if USE_WCF
            var ctx = WebOperationContext.Current;
            // ReSharper disable once PossibleNullReferenceException
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
            Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
            return new NotificationModel[0];
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/start")]
#endif
        public void Start(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StartModule(module);
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/stop")]
#endif
        public void Stop(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StopModule(module);
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/reincarnate")]
#endif
        public void Reincarnate(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ParallelOperations.ExecuteParallel(ModuleManager.ReincarnateModule, module);
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}")]
#endif
        public void Update(string moduleName, ServerModuleModel module)
        {
            var serverModule = GetModuleFromManager(moduleName);

            var startBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(serverModule);
            if (startBehaviour.Behaviour != module.StartBehaviour)
            {
                Logger.Log(LogLevel.Info, "Changing start behaviour of {0} to {1}", moduleName, module.StartBehaviour);
                startBehaviour.Behaviour = module.StartBehaviour;
            }

            var failureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(serverModule);
            if (failureBehaviour.Behaviour != module.FailureBehaviour)
            {
                Logger.Log(LogLevel.Info, "Changing failure behaviour of {0} to {1}", moduleName, module.FailureBehaviour);
                failureBehaviour.Behaviour = module.FailureBehaviour;
            }
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/confirm")]
#endif
        public void ConfirmWarning(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            foreach (var notification in module.Notifications.ToArray())
            {
                notification.Confirm();
            }
            ModuleManager.InitializeModule(module);
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("module/{moduleName}/config")]
#endif
        public Config GetConfig(string moduleName)
        {
            Logger.Log(LogLevel.Info, "Converting config of plugin {0}", moduleName);
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);

                var config = GetConfig(module, false);
                var configModel = new Config
                {
                    Root = EntryConvert.EncodeObject(config, serialization)
                };
                return configModel;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to convert config of {0}", moduleName);
#if USE_WCF
                var ctx = WebOperationContext.Current;
                // ReSharper disable once PossibleNullReferenceException
                ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
#else
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#endif
                return null;
            }
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/config")]
#endif
        public void SetConfig(string moduleName, SaveConfigRequest request)
        {
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, true);
                EntryConvert.UpdateInstance(config, request.Config.Root, serialization);
                ConfigManager.SaveConfiguration(config, request.UpdateMode == ConfigUpdateMode.UpdateLiveAndSave);

                if (request.UpdateMode == ConfigUpdateMode.SaveAndReincarnate)
                    // This has to be done parallel so we can also reincarnate the Maintenance itself
                    ParallelOperations.ExecuteParallel(() => ModuleManager.ReincarnateModule(module));
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to save config of {0}", moduleName);
#if USE_WCF
                var ctx = WebOperationContext.Current;
                // ReSharper disable once PossibleNullReferenceException
                ctx.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
#else
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#endif
            }
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpGet("module/{moduleName}/console")]
#endif
        public MethodEntry[] GetMethods(string moduleName)
        {
            var methods = new MethodEntry[] {};
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule?.Console != null)
            {
                methods = EntryConvert.EncodeMethods(serverModule.Console, CreateEditorSerializeSerialization(serverModule)).ToArray();
            }

            return methods;
        }

        /// <inheritdoc />
#if !USE_WCF
        [HttpPost("module/{moduleName}/console")]
#endif
        public Entry InvokeMethod(string moduleName, MethodEntry method)
        {
            Entry result = null;
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule != null && method != null)
            {
                try
                {
                    result = EntryConvert.InvokeMethod(serverModule.Console, method,
                        CreateEditorSerializeSerialization(serverModule));
                }
                catch (Exception e)
                {
                    result = new Entry
                    {
                        Description = $"Error while invoking function: {method.DisplayName}",
                        DisplayName = "Error description",
                        Identifier = "0",
                        Value = new EntryValue {Current = e.Message, Type = EntryValueType.String}
                    };
                }
            }
            else
            {
#if USE_WCF
            var ctx = WebOperationContext.Current;
            // ReSharper disable once PossibleNullReferenceException
            ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
#else
                Response.StatusCode = (int)HttpStatusCode.NotFound;
#endif
            }

            return result;
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateSerialization(IModule module)
        {
            var host = (IContainerHost) module;
            return new PossibleValuesSerialization(host.Container, ConfigManager)
            {
                FormatProvider = Thread.CurrentThread.CurrentUICulture
            };
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateEditorSerializeSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            return new AdvancedEntrySerializeSerialization(host.Container, ConfigManager)
            {
                FormatProvider = Thread.CurrentThread.CurrentUICulture
            };
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

            var assemblyVersion = assemblyName.Version;
            var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var informationalVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var model = new AssemblyModel
            {
                Name = assemblyName.Name + ".dll",
                Version = assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Build,
                FileVersion = fileVersionAttr?.Version,
                InformationalVersion = informationalVersionAttr?.InformationalVersion
            };
            return model;
        }
    }
}
