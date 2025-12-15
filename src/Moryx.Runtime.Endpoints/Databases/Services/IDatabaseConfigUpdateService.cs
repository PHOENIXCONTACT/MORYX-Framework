// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Models;

namespace Moryx.Runtime.Endpoints.Databases.Services
{
    public interface IDatabaseConfigUpdateService
    {
        Type UpdateModel(string targetModel, DatabaseConfigModel config);
    }
}
