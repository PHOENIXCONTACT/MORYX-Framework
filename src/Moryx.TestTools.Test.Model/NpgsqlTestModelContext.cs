// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.TestTools.Test.Model;

[NpgsqlDbContext(typeof(TestModelContext))]
public class NpgsqlTestModelContext : TestModelContext
{
    public NpgsqlTestModelContext()
    {
    }

    public NpgsqlTestModelContext(DbContextOptions options) : base(options)
    {
    }
}
