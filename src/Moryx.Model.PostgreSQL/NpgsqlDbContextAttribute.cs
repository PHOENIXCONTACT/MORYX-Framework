// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Attributes;

namespace Moryx.Model.PostgreSQL;

/// <summary>
/// Attribute to identify Npgsql specific contexts
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NpgsqlDbContextAttribute : DatabaseTypeSpecificDbContextAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDbContextAttribute"/> class.
    /// </summary>
    public NpgsqlDbContextAttribute() : base(typeof(NpgsqlModelConfigurator))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDbContextAttribute"/> class.
    /// </summary>
    /// <param name="baseDbContextType">Type of the base DbContext-type</param>
    public NpgsqlDbContextAttribute(Type baseDbContextType) : base(typeof(NpgsqlModelConfigurator), baseDbContextType)
    {

    }
}
