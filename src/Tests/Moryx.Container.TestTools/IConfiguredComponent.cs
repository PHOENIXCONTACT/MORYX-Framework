// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Container.TestTools
{
    public interface IConfiguredComponent : IConfiguredPlugin<ComponentConfig>
    {
        string GetName();
    }
}
