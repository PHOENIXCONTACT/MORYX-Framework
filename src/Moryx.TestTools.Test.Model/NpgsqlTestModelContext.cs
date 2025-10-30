// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.EntityFrameworkCore;
using Moryx.Model.PostgreSQL;

namespace Moryx.TestTools.Test.Model;

[NpgsqlDbContext]
public class NpgsqlTestModelContext : TestModelContext
{
    public NpgsqlTestModelContext()
    {
    }

    public NpgsqlTestModelContext(DbContextOptions options) : base(options)
    {
    }
}
