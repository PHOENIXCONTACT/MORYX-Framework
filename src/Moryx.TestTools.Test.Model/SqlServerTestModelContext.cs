// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Attributes;
using Moryx.Model.SqlServer;
using Moryx.TestTools.Test.Model;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model;

/// <summary>
/// SqlServer specific implementation of <see cref="TestModelContext"/>
/// </summary>
[SqlServerDbContext(typeof(TestModelContext))]
[DefaultSchema("testmodel")]
public class SqlServerTestModelContext : TestModelContext
{
    /// <inheritdoc />
    public SqlServerTestModelContext()
    {
    }

    /// <inheritdoc />
    public SqlServerTestModelContext(DbContextOptions options) : base(options)
    {
    }
}
