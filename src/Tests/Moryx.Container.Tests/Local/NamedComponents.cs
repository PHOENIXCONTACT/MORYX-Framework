// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    public interface INamedComponent
    {
        string GetName();
    }

    [Plugin(LifeCycle.Transient, typeof(INamedComponent), Name = ComponentName)]
    public class NamedComponentA : INamedComponent
    {
        internal const string ComponentName = "CompA";

        public string GetName()
        {
            return ComponentName;
        }
    }

    [Plugin(LifeCycle.Transient, typeof(INamedComponent), Name = ComponentName)]
    public class NamedComponentB : INamedComponent
    {
        internal const string ComponentName = "CompB";

        public string GetName()
        {
            return ComponentName;
        }
    }

    [PluginFactory(typeof(INameBasedComponentSelector))]
    public interface INamedComponentFactory
    {
        INamedComponent Create(string name);
    }
}
