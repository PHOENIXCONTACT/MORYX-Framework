// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Moryx.Communication;

namespace Moryx.Tools.Wcf
{
    internal interface IVersionServiceManager : IDisposable
    {
        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Endpoint[] ActiveEndpoints();

        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Task<Endpoint[]> ActiveEndpointsAsync();

        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Endpoint[] ServiceEndpoints(string service);

        /// <summary>
        /// Available endpoints for this service type
        /// </summary>
        Task<Endpoint[]> ServiceEndpointsAsync(string service);
    }
}
