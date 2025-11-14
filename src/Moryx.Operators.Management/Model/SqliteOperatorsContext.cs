// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.Operators.Management.Model;

/// <summary>
/// Sqlite specific implementation of <see cref="OperatorsContext"/>
/// </summary>
[SqliteDbContext(typeof(OperatorsContext))]
public class SqliteOperatorsContext : OperatorsContext
{
    /// <inheritdoc />
    public SqliteOperatorsContext()
    {
    }

    /// <inheritdoc />
    public SqliteOperatorsContext(DbContextOptions options) : base(options)
    {
    }
}

