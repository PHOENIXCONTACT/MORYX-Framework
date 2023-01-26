// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.TestModule
{
    public interface IAnotherPlugin : IConfiguredPlugin<AnotherPluginConfig>
    {
         
    }

    public interface IAnotherSubPlugin : IConfiguredPlugin<AnotherSubConfig>
    {
        
    }
}
