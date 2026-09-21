// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

internal static class ABehaviourAccess
{
    public static ABehaviourAccess<T> Create<T>(ModuleManagerConfig config, IConfigManager configManager, IModule module)
    {
        if (typeof(T) == typeof(ModuleStartBehaviour))
            return new StartBehaviorAccess(config, configManager, module) as ABehaviourAccess<T>;

        if (typeof(T) == typeof(FailureBehaviour))
            return new FailureBehaviourAccess(config, configManager, module) as ABehaviourAccess<T>;

        return null;
    }
}

internal abstract class ABehaviourAccess<T> : IBehaviourAccess<T>
{
    private readonly ModuleManagerConfig _config;
    private readonly IConfigManager _configManager;

    protected ManagedModuleConfig Module { get; }

    protected ABehaviourAccess(ModuleManagerConfig config, IConfigManager configManager, IModule module)
    {
        _config = config;
        _configManager = configManager;

        Module = config.GetOrCreate(module.Name);
    }

    /// <summary>
    /// Get or set the services behaviour
    /// </summary>
    public T Behaviour
    {
        get { return GetBehavior(); }
        set
        {
            SetBehavior(value);
            _configManager.SaveConfiguration(_config);
        }
    }

    protected abstract T GetBehavior();

    protected abstract void SetBehavior(T behavior);
}

internal class StartBehaviorAccess : ABehaviourAccess<ModuleStartBehaviour>
{
    public StartBehaviorAccess(ModuleManagerConfig config, IConfigManager configManager, IModule module) : base(config, configManager, module)
    {
    }

    protected override ModuleStartBehaviour GetBehavior()
    {
        return Module.StartBehaviour;
    }

    protected override void SetBehavior(ModuleStartBehaviour behavior)
    {
        Module.StartBehaviour = behavior;
    }
}

internal class FailureBehaviourAccess : ABehaviourAccess<FailureBehaviour>
{
    public FailureBehaviourAccess(ModuleManagerConfig config, IConfigManager configManager, IModule module) : base(config, configManager, module)
    {
    }

    protected override FailureBehaviour GetBehavior()
    {
        return Module.FailureBehaviour;
    }

    protected override void SetBehavior(FailureBehaviour behavior)
    {
        Module.FailureBehaviour = behavior;
    }
}
