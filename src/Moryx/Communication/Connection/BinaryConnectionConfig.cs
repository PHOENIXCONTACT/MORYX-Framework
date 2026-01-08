// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Communication
{
    /// <summary>
    /// Base config for client and server config modes
    /// </summary>
    public class BinaryConnectionConfig : IPluginConfig
    {
        ///
        public virtual string PluginName => "CommunicatorConfig";
    }
}
