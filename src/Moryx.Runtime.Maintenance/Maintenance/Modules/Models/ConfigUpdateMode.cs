// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Modules
{
    /// <summary>
    /// Enum for the update behavior of a plugin on a configuration save.
    /// </summary>
    public enum ConfigUpdateMode
    {
        /// <summary>
        /// Only save changes and use them with next plugin start
        /// </summary>
        OnlySave,

        /// <summary>
        /// Save changes and reincarnate plugin so changes take effect
        /// </summary>
        SaveAndReincarnate,

        /// <summary>
        /// Update current config object live while plugin is still running
        /// </summary>
        UpdateLiveAndSave
    }
}
