// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Data.Entity;

namespace Moryx.Model
{
    internal interface IDbContextFactory
    {
        DbContext CreateContext(ContextMode contextMode);

        DbContext CreateContext(IDatabaseConfig config, ContextMode contextMode);
    }
}
