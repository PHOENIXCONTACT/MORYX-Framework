// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Models;

namespace Moryx.Runtime.Endpoints.Databases.Response;

public class DatabasesResponse
{
    public DataModel[] Databases { get; set; }

    public ModelConfiguratorModel[] Configurators { get; set; }
}