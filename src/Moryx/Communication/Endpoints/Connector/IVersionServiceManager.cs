// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Service manager api for provide active endpoints of the current application runtime
    /// </summary>
    public interface IVersionServiceManager : IDisposable
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
