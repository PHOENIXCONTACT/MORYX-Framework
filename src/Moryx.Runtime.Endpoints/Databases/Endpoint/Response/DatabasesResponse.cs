// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Response
{
    public class DatabasesResponse
    {
        public DataModel[] Databases { get; set; }
    }
}

