// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Container.Tests
{
    public interface ILocalComponent
    {
    }

    [Plugin(LifeCycle.Transient, typeof(ILocalComponent))]
    internal class LocalComponent : ILocalComponent
    {
    }

    [PluginFactory]
    public interface ILocalFactory
    {
        ILocalComponent Create();
    }
}
