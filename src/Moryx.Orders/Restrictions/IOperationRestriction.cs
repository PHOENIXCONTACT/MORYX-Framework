// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Restrictions
{
    public interface IOperationRestriction
    {
        RestrictionDescription Description { get; }
    }
}