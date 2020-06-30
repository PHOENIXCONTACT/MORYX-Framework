// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Modules;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    internal static class ABehaviourAccess
    {
        public static ABehaviourAccess<T> Create<T>(ModuleManagerConfig config, IModule module)
        {
            if (typeof(T) == typeof(ModuleStartBehaviour))
                return new StartBehaviorAccess(config, module) as ABehaviourAccess<T>;

            if (typeof(T) == typeof(FailureBehaviour))
                return new FailureBehaviourAccess(config, module) as ABehaviourAccess<T>;

            return null;
        }
    }

    internal abstract class ABehaviourAccess<T> : IBehaviourAccess<T>
    {
        protected ManagedModuleConfig Module { get; private set; }
        protected ABehaviourAccess(ModuleManagerConfig config, IModule module)
        {
            Module = config.GetOrCreate(module.Name);
        }

        /// <summary>
        /// Get or set the services behaviour
        /// </summary>
        public T Behaviour
        {
            get { return GetBehavior(); }
            set { SetBehavior(value); }
        }

        protected abstract T GetBehavior();

        protected abstract void SetBehavior(T behavior);
    }

    internal class StartBehaviorAccess : ABehaviourAccess<ModuleStartBehaviour>
    {
        public StartBehaviorAccess(ModuleManagerConfig config, IModule module) : base(config, module)
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
        public FailureBehaviourAccess(ModuleManagerConfig config, IModule module) : base(config, module)
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
}
