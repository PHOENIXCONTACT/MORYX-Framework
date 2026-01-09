// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Attributes;

namespace Moryx.Model.Sqlite;

/// <summary>
/// Attribute to identify Sqlite specific contexts
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SqliteDbContextAttribute : DatabaseTypeSpecificDbContextAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDbContextAttribute"/> class.
    /// </summary>
    public SqliteDbContextAttribute() : base(typeof(SqliteModelConfigurator))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteDbContextAttribute"/> class.
    /// </summary>
    /// <param name="baseDbContextType">Type of the base DbContext-type</param>
    public SqliteDbContextAttribute(Type baseDbContextType) : base(typeof(SqliteModelConfigurator), baseDbContextType)
    {
    }
}
