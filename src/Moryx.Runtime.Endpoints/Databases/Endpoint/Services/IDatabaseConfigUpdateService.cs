// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;
using System;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Services
{
    public interface IDatabaseConfigUpdateService
    {
        Type UpdateModel(string targetModel, DatabaseConfigModel config);
    }
}
