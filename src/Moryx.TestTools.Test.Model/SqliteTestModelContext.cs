// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.Sqlite;

namespace Moryx.TestTools.Test.Model;

[SqliteDbContext(typeof(TestModelContext))]
public class SqliteTestModelContext : TestModelContext
{
    public SqliteTestModelContext()
    {
    }

    public SqliteTestModelContext(DbContextOptions options) : base(options)
    {
    }
}
