// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.TestTools
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IRootClassFactory
    {
        IRootClass Create(RootClassFactoryConfig config);

        void Destroy(IRootClass instance);
    }
}
