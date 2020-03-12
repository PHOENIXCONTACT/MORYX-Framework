// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Container.TestTools
{
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    public interface IRootClassFactory
    {
        IRootClass Create(RootClassFactoryConfig config);

        void Destroy(IRootClass instance);
    }
}
