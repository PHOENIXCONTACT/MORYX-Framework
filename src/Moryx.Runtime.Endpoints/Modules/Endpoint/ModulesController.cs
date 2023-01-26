// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Runtime.Endpoints.Modules.Endpoint.Models;
using Moryx.Runtime.Endpoints.Modules.Endpoint.Request;
using Moryx.Runtime.Endpoints.Modules.Serialization;
using Moryx.Threading;

namespace Moryx.Runtime.Endpoints.Modules.Endpoint
{
    [ApiController]
    [Route("modules")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleManager _moduleManager;
        private readonly IConfigManager _configManager;
        private readonly IParallelOperations _parallelOperations;

        public ModulesController(IModuleManager moduleManager, IConfigManager configManager, IParallelOperations parallelOperations)
        {
            _moduleManager = moduleManager;
            _configManager = configManager;
            _parallelOperations = parallelOperations;
        }

        [HttpGet("dependencies")]
        public ActionResult<DependencyEvaluation> GetDependencyEvaluation()
            => new DependencyEvaluation(_moduleManager.DependencyTree);


        [HttpGet]
        public ActionResult<IEnumerable<ServerModuleModel>> GetAll()
        {
            var models = new List<ServerModuleModel>(_moduleManager.AllModules.Count());
            foreach (var module in _moduleManager.AllModules)
            {
                var notifications = module.Notifications.ToArray();

                var model = new ServerModuleModel
                {
                    Name = module.Name,
                    Assembly = ConvertAssembly(module),
                    HealthState = module.State,
                    StartBehaviour = _moduleManager.BehaviourAccess<ModuleStartBehaviour>(module).Behaviour,
                    FailureBehaviour = _moduleManager.BehaviourAccess<FailureBehaviour>(module).Behaviour,
                    Notifications = notifications.Select(n => new ModuleNotificationModel(n)).ToArray()
                };

                var dependencies = _moduleManager.StartDependencies(module);
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
            return models;
        }

        [HttpGet("{moduleName}/healthstate")]
        public ActionResult<ServerModuleState> HealthState([FromRoute] string moduleName)
        {
            var module = _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
                return module.State;

            return NotFound($"Module with name \"{moduleName}\" could not be found");
        }

        [HttpGet("{moduleName}/notifications")]
        public ActionResult<IEnumerable<ModuleNotificationModel>> Notifications([FromRoute] string moduleName)
        {
            var module = _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
                return Ok(module.Notifications.Select(n => new ModuleNotificationModel(n)));

            return NotFound($"Module with name \"{moduleName}\" could not be found");
        }

        [HttpPost("{moduleName}/start")]
        public ActionResult Start([FromRoute] string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            if (module == null)
                return NotFound($"Module with name \"{moduleName}\" could not be found");

            _moduleManager.StartModule(module);
            return Ok();
        }

        [HttpPost("{moduleName}/stop")]
        public ActionResult Stop([FromRoute] string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            if (module == null)
                return NotFound($"Module with name \"{moduleName}\" could not be found");

            _moduleManager.StopModule(module);
            return Ok();
        }

        [HttpPost("{moduleName}/reincarnate")]
        public ActionResult Reincarnate([FromRoute] string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            if (module == null)
                return NotFound($"Module with name \"{moduleName}\" could not be found");

            _parallelOperations.ExecuteParallel(_moduleManager.ReincarnateModule, module);
            return Ok();
        }

        [HttpPost("{moduleName}")]
        public ActionResult Update([FromRoute] string moduleName, [FromBody] ServerModuleModel module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            var serverModule = GetModuleFromManager(moduleName);
            if (serverModule == null)
                return NotFound($"Server module with name \"{moduleName}\" could not be found");

            var startBehaviour = _moduleManager.BehaviourAccess<ModuleStartBehaviour>(serverModule);
            if (startBehaviour.Behaviour != module.StartBehaviour)
                startBehaviour.Behaviour = module.StartBehaviour;

            var failureBehaviour = _moduleManager.BehaviourAccess<FailureBehaviour>(serverModule);
            if (failureBehaviour.Behaviour != module.FailureBehaviour)
                failureBehaviour.Behaviour = module.FailureBehaviour;

            return Ok();
        }

        [HttpPost("{moduleName}/confirm")]
        public ActionResult ConfirmWarning([FromRoute] string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            if (module == null)
                return NotFound($"Module with name \"{moduleName}\" could not be found");

            var notifications = module.Notifications.ToArray();
            foreach (var notification in notifications)
                module.AcknowledgeNotification(notification);

            _moduleManager.InitializeModule(module);
            return Ok();
        }

        [HttpGet("{moduleName}/config")]
        public ActionResult<Config> GetConfig([FromRoute] string moduleName)
        {
            try
            {
                var module = GetModuleFromManager(moduleName);
                if (module == null)
                    return NotFound($"Module with name \"{moduleName}\" could not be found");

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
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("{moduleName}/config")]
        public ActionResult SetConfig([FromRoute] string moduleName, [FromBody] SaveConfigRequest request)
        {
            if (request.Config == null)
                throw new ArgumentNullException(nameof(request.Config));

            try
            {
                var module = GetModuleFromManager(moduleName);
                if (module == null)
                    return NotFound($"Module with name \"{moduleName}\" could not be found");

                var serialization = CreateSerialization(module);
                var config = GetConfig(module, true);
                EntryConvert.UpdateInstance(config, request.Config.Root, serialization);
                _configManager.SaveConfiguration(config, request.UpdateMode == ConfigUpdateMode.UpdateLiveAndSave);

                if (request.UpdateMode == ConfigUpdateMode.SaveAndReincarnate)
                    // This has to be done parallel so we can also reincarnate the Maintenance itself
                    _parallelOperations.ExecuteParallel(() => _moduleManager.ReincarnateModule(module));
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("{moduleName}/console")]
        public ActionResult<IEnumerable<MethodEntry>> GetMethods([FromRoute] string moduleName)
        {
            var methods = Enumerable.Empty<MethodEntry>();
            var serverModule = GetModuleFromManager(moduleName);
            if (serverModule == null)
                return NotFound($"Server module with name \"{moduleName}\" could not be found");

            if (serverModule?.Console != null)
                methods = EntryConvert.EncodeMethods(serverModule.Console, CreateEditorSerializeSerialization(serverModule));

            return Ok(methods);
        }

        [HttpPost("{moduleName}/console")]
        public ActionResult<Entry> InvokeMethod([FromRoute] string moduleName, [FromBody] MethodEntry method)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            var serverModule = GetModuleFromManager(moduleName);
            if (serverModule == null)
                return NotFound($"Server module with name \"{moduleName}\" could not be found");

            try
            {
                return EntryConvert.InvokeMethod(serverModule.Console, method,
                    CreateEditorSerializeSerialization(serverModule));
            }
            catch (Exception e)
            {
                throw new Exception($"Error while invoking function: {method.DisplayName}", e);
            }
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            // TODO: This is dangerous
            return new PossibleValuesSerialization(host.Container, (IEmptyPropertyProvider)_configManager)
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
            // TODO: This is dangerous
            return new AdvancedEntrySerializeSerialization(host.Container, (IEmptyPropertyProvider)_configManager)
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

            return _configManager.GetConfiguration(configType, copy);
        }

        private IServerModule GetModuleFromManager(string moduleName)
            => _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);

        private static AssemblyModel ConvertAssembly(IInitializable service)
        {
            var assembly = service.GetType().Assembly;
            var assemblyName = assembly.GetName();

            var assemblyVersion = assemblyName.Version;
            var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var informationalVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return new AssemblyModel
            {
                Name = assemblyName.Name + ".dll",
                Version = assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Build,
                FileVersion = fileVersionAttr?.Version,
                InformationalVersion = informationalVersionAttr?.InformationalVersion
            };
        }
    }
}
