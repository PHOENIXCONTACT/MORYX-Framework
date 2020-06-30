// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Model
{
    internal interface IContextUnitOfWorkFactory
    {
        IUnitOfWork Create(MoryxDbContext context);
    }
}
