// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Runtime.Endpoints.Modules.Models;
using Moryx.Runtime.Endpoints.Modules.Request;
using Moryx.Runtime.Endpoints.Modules.Serialization;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Modules;

[ApiController]
[Route("modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleManager _moduleManager;
    private readonly IConfigManager _configManager;
    private readonly IServiceProvider _serviceProvider;

    public ModulesController(IModuleManager moduleManager, IConfigManager configManager, IServiceProvider serviceProvider)
    {
        _moduleManager = moduleManager;
        _configManager = configManager;
        _serviceProvider = serviceProvider;
    }

    [HttpGet("dependencies")]
    public ActionResult<DependencyEvaluation> GetDependencyEvaluation()
        => new DependencyEvaluation(_moduleManager.DependencyTree);

    [HttpGet]
    [Authorize(Policy = RuntimePermissions.ModulesCanView)]
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
    [Authorize(Policy = RuntimePermissions.ModulesCanView)]
    public ActionResult<ServerModuleState> HealthState([FromRoute] string moduleName)
    {
        var module = _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
        if (module != null)
            return module.State;

        return NotFound($"Module with name \"{moduleName}\" could not be found");
    }

    [HttpGet("{moduleName}/notifications")]
    [Authorize(Policy = RuntimePermissions.ModulesCanView)]
    public ActionResult<IEnumerable<ModuleNotificationModel>> Notifications([FromRoute] string moduleName)
    {
        var module = _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
        if (module != null)
            return Ok(module.Notifications.Select(n => new ModuleNotificationModel(n)));

        return NotFound($"Module with name \"{moduleName}\" could not be found");
    }

    [HttpPost("{moduleName}/start")]
    [Authorize(Policy = RuntimePermissions.ModulesCanControl)]
    public ActionResult Start([FromRoute] string moduleName)
    {
        var module = GetModuleFromManager(moduleName);
        if (module == null)
            return NotFound($"Module with name \"{moduleName}\" could not be found");

        _moduleManager.StartModuleAsync(module);
        return Ok();
    }

    [HttpPost("{moduleName}/stop")]
    [Authorize(Policy = RuntimePermissions.ModulesCanControl)]
    public ActionResult Stop([FromRoute] string moduleName)
    {
        var module = GetModuleFromManager(moduleName);
        if (module == null)
            return NotFound($"Module with name \"{moduleName}\" could not be found");

        _moduleManager.StopModuleAsync(module);
        return Ok();
    }

    [HttpPost("{moduleName}/reincarnate")]
    [Authorize(Policy = RuntimePermissions.ModulesCanControl)]
    public ActionResult Reincarnate([FromRoute] string moduleName)
    {
        var module = GetModuleFromManager(moduleName);
        if (module == null)
            return NotFound($"Module with name \"{moduleName}\" could not be found");

        Task.Run(() => _moduleManager.ReincarnateModuleAsync(module));
        return Ok();
    }

    [HttpPost("{moduleName}")]
    [Authorize(Policy = RuntimePermissions.ModulesCanConfigure)]
    public ActionResult Update([FromRoute] string moduleName, [FromBody] ServerModuleModel module)
    {
        ArgumentNullException.ThrowIfNull(module);

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
    [Authorize(Policy = RuntimePermissions.ModulesCanConfirmNotifications)]
    public ActionResult ConfirmWarning([FromRoute] string moduleName)
    {
        var module = GetModuleFromManager(moduleName);
        if (module == null)
            return NotFound($"Module with name \"{moduleName}\" could not be found");

        var notifications = module.Notifications.ToArray();
        foreach (var notification in notifications)
            module.AcknowledgeNotification(notification);

        _moduleManager.InitializeModuleAsync(module);
        return Ok();
    }

    [HttpGet("{moduleName}/config")]
    [Authorize(Policy = RuntimePermissions.ModulesCanViewConfiguration)]
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
    [Authorize(Policy = RuntimePermissions.ModulesCanConfigure)]
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
                Task.Run(() => _moduleManager.ReincarnateModuleAsync(module));
            return Ok();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{moduleName}/console")]
    [Authorize(Policy = RuntimePermissions.ModulesCanViewMethods)]
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
    [Authorize(Policy = RuntimePermissions.ModulesCanInvoke)]
    public async Task<ActionResult<Entry>> InvokeMethod([FromRoute] string moduleName, [FromBody] MethodEntry method)
    {
        ArgumentNullException.ThrowIfNull(method);

        var serverModule = GetModuleFromManager(moduleName);
        if (serverModule == null)
            return NotFound($"Server module with name \"{moduleName}\" could not be found");

        try
        {
            if (method.IsAsync)
            {
                return await EntryConvert.InvokeMethodAsync(serverModule.Console, method,
                    CreateEditorSerializeSerialization(serverModule));
            }
            else
            {
                // ReSharper disable once MethodHasAsyncOverload
                return EntryConvert.InvokeMethod(serverModule.Console, method,
                    CreateEditorSerializeSerialization(serverModule));
            }

        }
        catch (Exception e)
        {
            throw new Exception($"Error while invoking function: {method.DisplayName}", e);
        }
    }

    /// <summary>
    /// Create serialization for this module
    /// </summary>
    private ICustomSerialization CreateSerialization(IServerModule module)
    {
        return new PossibleValuesSerialization(module.Container, _serviceProvider, (IEmptyPropertyProvider)_configManager)
        {
            FormatProvider = Thread.CurrentThread.CurrentUICulture
        };
    }

    /// <summary>
    /// Create serialization for this module
    /// </summary>
    private ICustomSerialization CreateEditorSerializeSerialization(IServerModule module)
    {
        return new AdvancedEntrySerializeSerialization(module.Container, _serviceProvider, (IEmptyPropertyProvider)_configManager)
        {
            FormatProvider = Thread.CurrentThread.CurrentUICulture
        };
    }

    /// <summary>
    /// Get the config type
    /// </summary>
    /// <returns></returns>
    private ConfigBase GetConfig(IModule module, bool copy)
    {
        var moduleType = module.GetType();
        var configType = moduleType.BaseType != null && moduleType.BaseType.IsGenericType
            ? moduleType.BaseType.GetGenericArguments()[0]
            : moduleType.Assembly.GetTypes().FirstOrDefault(type => typeof(ConfigBase).IsAssignableFrom(type));

        return _configManager.GetConfiguration(configType, copy);
    }

    private IServerModule GetModuleFromManager(string moduleName)
        => _moduleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);

    private static AssemblyModel ConvertAssembly(IAsyncInitializable service)
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
