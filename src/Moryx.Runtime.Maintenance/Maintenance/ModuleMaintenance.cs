// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Maintenance.Modules;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Threading;
using Nancy;
using Nancy.ModelBinding;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace Moryx.Runtime.Maintenance
{
    [Plugin(LifeCycle.Singleton, typeof(INancyModule), typeof(ModuleMaintenance))]
    internal sealed class ModuleMaintenance : NancyModule,  ILoggingComponent
    {
        #region dependency injection

        public IModuleManager ModuleManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IRuntimeConfigManager ConfigManager { get; set; }

        [UseChild("ModuleMaintenance")]
        public IModuleLogger Logger { get; set; }

        #endregion

        public ModuleMaintenance() : base("modules")
        {
            Get("/", args => Response.AsJson(GetAll()));

            Get("dependencyEvaluation", args => Response.AsJson(GetDependencyEvaluation()));

            Get("{moduleName}/healthState", delegate(dynamic args)
            {
                var moduleName = (string)args.moduleName;
                return Response.AsJson(HealthState(moduleName));
            });

            Get("{moduleName}/notifications", delegate (dynamic args)
            {
                var moduleName = (string)args.moduleName;
                return Response.AsJson(Notifications(moduleName));
            });

            Post("{moduleName}", delegate (dynamic args)
            {
                var moduleModel = this.Bind<ServerModuleModel>();
                Update(args.moduleName, moduleModel);
                return HttpStatusCode.OK;
            });

            Post("{moduleName}/start", delegate(dynamic args)
            {
                Start(args.moduleName);
                return HttpStatusCode.OK;

            });

            Post("{moduleName}/stop", delegate (dynamic args)
            {
                Stop(args.moduleName);
                return HttpStatusCode.OK;
            });

            Post("{moduleName}/confirm", delegate (dynamic args)
            {
                ConfirmWarning(args.moduleName);
                return HttpStatusCode.OK;
            });

            Post("{moduleName}/reincarnate", delegate (dynamic args)
            {
                Reincarnate(args.moduleName);
                return HttpStatusCode.OK;
            });

            Get("{moduleName}/config", delegate (dynamic args)
            {
                var moduleName = (string)args.moduleName;
                var config = GetConfig(moduleName);
                return Response.AsJson(config, config != null ? Nancy.HttpStatusCode.OK : Nancy.HttpStatusCode.NotFound);
            });

            Post("{moduleName}/config", delegate (dynamic args)
            {
                var config = this.Bind<SaveConfigRequest>();
                SetConfig(args.moduleName, config);
                return Response.AsJson(config, config != null ? Nancy.HttpStatusCode.OK : Nancy.HttpStatusCode.NotFound);
            });

            Get("{moduleName}/console", delegate (dynamic args)
            {
                var moduleName = (string)args.moduleName;
                var methods = GetMethods(moduleName);
                return Response.AsJson(methods, methods != null ? Nancy.HttpStatusCode.OK : Nancy.HttpStatusCode.NotFound);
            });

            Post("{moduleName}/console", delegate (dynamic args)
            {
                var method = this.Bind<MethodEntry>();
                var result = InvokeMethod((string)args.moduleName, method);
                return Response.AsJson(result, result != null ? Nancy.HttpStatusCode.OK : Nancy.HttpStatusCode.NotFound);
            });
        }

        private ServerModuleModel[] GetAll()
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

        private ServerModuleState HealthState(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            return module?.State ?? ServerModuleState.Failure;
        }

        private NotificationModel[] Notifications(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            return module != null ? module.Notifications.Select(n => new NotificationModel(n)).ToArray() : new NotificationModel[0];
        }

        private DependencyEvaluation GetDependencyEvaluation()
        {
            return new DependencyEvaluation(ModuleManager.DependencyEvaluation);
        }

        private void Start(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StartModule(module);
        }

        private void Stop(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StopModule(module);
        }

        private void Reincarnate(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ParallelOperations.ExecuteParallel(ModuleManager.ReincarnateModule, module);
        }

        private void Update(string moduleName, ServerModuleModel module)
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

        private void ConfirmWarning(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            foreach (var notification in module.Notifications.ToArray())
            {
                notification.Confirm();
            }
            ModuleManager.InitializeModule(module);
        }

        private Config GetConfig(string moduleName)
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
                //HttpHelper.SetStatusCode(HttpStatusCode.InternalServerError);
                return null;
            }
        }

        private void SetConfig(string moduleName, SaveConfigRequest request)
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
            }
        }

        private MethodEntry[] GetMethods(string moduleName)
        {
            var methods = new MethodEntry[] {};
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule?.Console != null)
            {
                methods = EntryConvert.EncodeMethods(serverModule.Console, CreateEditorBrowsableSerialization(serverModule)).ToArray();
            }

            return methods;
        }

        private Entry InvokeMethod(string moduleName, MethodEntry method)
        {
            Entry result = null;
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule != null && method != null)
            {
                try
                {
                    result = EntryConvert.InvokeMethod(serverModule.Console, method,
                        CreateEditorBrowsableSerialization(serverModule));
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
                //var ctx = WebOperationContext.Current;
                //// ReSharper disable once PossibleNullReferenceException
                //ctx.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
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
        private ICustomSerialization CreateEditorBrowsableSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            return new AdvancedEditorBrowsableSerialization(host.Container, ConfigManager)
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
