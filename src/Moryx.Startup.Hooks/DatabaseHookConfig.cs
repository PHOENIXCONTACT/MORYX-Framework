// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Startup.Hooks;

public class DatabaseHookConfig
{
    /// <summary>
    /// Allows disabling this config entry
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Delete all dbs on startup
    /// </summary>
    public bool DeleteAllDbs { get; set; }

    /// <summary>
    /// Allows deleting only specific databases by the context name
    /// </summary>
    public string[] DbsToDeleteOnStartup { get; set; } = [];

    /// <summary>
    /// Create all missing dbs
    /// </summary>
    public bool CreateDbs { get; set; }
}
