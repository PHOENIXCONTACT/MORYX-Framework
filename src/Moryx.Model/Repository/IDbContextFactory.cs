// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Model
{
    internal interface IDbContextFactory
    {
        MarvinDbContext CreateContext(ContextMode contextMode);

        MarvinDbContext CreateContext(IDatabaseConfig config, ContextMode contextMode);
    }
}
