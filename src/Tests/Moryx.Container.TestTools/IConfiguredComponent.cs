// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Modules;

namespace Marvin.Container.TestTools
{
    public interface IConfiguredComponent : IConfiguredPlugin<ComponentConfig>
    {
        string GetName();
    }
}
