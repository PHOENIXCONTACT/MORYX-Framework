// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moryx.Communication.Endpoints;

namespace Moryx.Runtime.Kestrel
{
    internal class EndpointCollector
    {
        private readonly ICollection<Endpoint> _endpoints = new List<Endpoint>();

        public Endpoint[] AllEndpoints
        {
            get
            {
                lock (_endpoints)
                {
                    return _endpoints.ToArray();
                }
            }
        }

        public void AddEndpoint(Endpoint endpoint)
        {
            lock (_endpoints)
            {
                _endpoints.Add(endpoint);
            }
        }
    }
}
