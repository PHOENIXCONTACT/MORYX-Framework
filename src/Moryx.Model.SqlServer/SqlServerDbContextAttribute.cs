// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Attributes;

namespace Moryx.Model.SqlServer;

/// <summary>
/// Attribute to identify SqlServer specific contexts
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SqlServerDbContextAttribute : DatabaseTypeSpecificDbContextAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDbContextAttribute"/> class.
    /// </summary>
    public SqlServerDbContextAttribute() : base(typeof(SqlServerModelConfigurator))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDbContextAttribute"/> class.
    /// </summary>
    /// <param name="baseDbContextType">Type of the base DbContext-type</param>
    public SqlServerDbContextAttribute(Type baseDbContextType) : base(typeof(SqlServerModelConfigurator), baseDbContextType)
    {
    }
}
