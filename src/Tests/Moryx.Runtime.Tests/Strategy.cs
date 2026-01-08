// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Runtime.Tests
{
    public interface IStrategy
    {
    }

    public class StrategyConfig : IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        public string PluginName { get; set; }
    }
}
