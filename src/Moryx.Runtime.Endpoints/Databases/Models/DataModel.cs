// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Databases.Models;

/// <summary>
/// Contains the data to the target model.
/// </summary>
public class DataModel
{
    /// <summary>
    /// Name of the target model..
    /// </summary>
    public string TargetModel { get; set; }

    /// <summary>
    /// Configuration of the database model.
    /// </summary>
    public DatabaseConfigModel Config { get; set; }

    /// <summary>
    /// Configurators that could be set up
    /// </summary>
    public string[] PossibleConfigurators { get; set; }

    /// <summary>
    /// An amount of setups for this model.
    /// </summary>
    public SetupModel[] Setups { get; set; }

    /// <summary>
    /// Available migrations for this context
    /// </summary>
    public DbMigrationsModel[] AvailableMigrations { get; set; }

    /// <summary>
    /// Installed migrations for this context
    /// </summary>
    public DbMigrationsModel[] AppliedMigrations { get; set; }
}