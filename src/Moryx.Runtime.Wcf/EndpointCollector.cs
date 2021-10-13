// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Wcf
{
    internal class EndpointCollector
    {
        private readonly Dictionary<string, Endpoint> _endpoints = new Dictionary<string, Endpoint>();

        public Endpoint[] AllEndpoints
        {
            get
            {
                lock (_endpoints)
                {
                    return _endpoints.Values.ToArray();
                }
            }
        }

        public void AddEndpoint(string address, Endpoint endpoint)
        {
            lock (_endpoints)
            {
                _endpoints[address] = endpoint;
            }
        }

        public void RemoveEndpoint(string address)
        {
            lock (_endpoints)
            {
                _endpoints.Remove(address);
            }
        }
    }
}
