// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Modules
{
    /// <summary>
    /// Interface for all plugin configurations
    /// </summary>
    public interface IPluginConfig
    {
        /// <summary>
        /// Name of the component represented by this entry
        /// </summary>
        string PluginName { get; }
    }
}
