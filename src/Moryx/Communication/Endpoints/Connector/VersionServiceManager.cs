// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Newtonsoft.Json;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Service manager base class for provide active endpoints of the current application runtime
    /// </summary>
    public abstract class VersionServiceManager<TEndpoint> : IVersionServiceManager, IProxyConfigAccess
        where TEndpoint : Endpoint
    {
        private const string ServiceName = "endpoints";

        /// <summary>
        /// Underlying http client for the requests
        /// </summary>
        protected HttpClient Client { get; set; }

        /// <inheritdoc/>
        public IProxyConfig ProxyConfig { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="VersionServiceManager{TEndpoint}"/>
        /// </summary>
        protected VersionServiceManager(IProxyConfig proxyConfig, string host, int port)
        {
            ProxyConfig = proxyConfig;
            Client = HttpClientBuilder.GetClient($"http://{host}:{port}/{ServiceName}/", ProxyConfig);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            Client.Dispose();
            Client = null;
        }

        /// <inheritdoc />
        public virtual Endpoint[] ActiveEndpoints()
        {
            try
            {
                return ActiveEndpointsAsync().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual async Task<Endpoint[]> ActiveEndpointsAsync()
        {
            var response = await Client.GetStringAsync("");
            return JsonConvert.DeserializeObject<TEndpoint[]>(response).ToArray<Endpoint>();
        }


        /// <inheritdoc />
        public virtual Endpoint[] ServiceEndpoints(string service)
        {
            try
            {
                return ServiceEndpointsAsync(service).Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual async Task<Endpoint[]> ServiceEndpointsAsync(string service)
        {
            var response = await Client.GetStringAsync($"service/{service}");
            return JsonConvert.DeserializeObject<TEndpoint[]>(response).ToArray<Endpoint>();
        }
    }
}
