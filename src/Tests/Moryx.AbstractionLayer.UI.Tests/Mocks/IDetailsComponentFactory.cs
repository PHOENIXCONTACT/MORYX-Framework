// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;

namespace Marvin.AbstractionLayer.UI.Tests
{
    [PluginFactory(typeof(DetailsComponentSelector))]
    public interface IDetailsComponentFactory : IDetailsFactory<IDetailsComponent>
    {
        
    }
}
