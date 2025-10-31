// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.Operators.Management.Model;

/// <inheritdoc />
[NpgsqlDbContext(typeof(OperatorsContext))]
public class NpgsqlOperatorsContext : OperatorsContext
{
    /// <inheritdoc />
    public NpgsqlOperatorsContext()
    {
    }

    /// <inheritdoc />
    public NpgsqlOperatorsContext(DbContextOptions options) : base(options)
    {
    }
}

