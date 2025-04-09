// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Helper class to get a configured HttpClient
    /// </summary>
    public static class HttpClientBuilder
    {
        /// <summary>
        /// Creates a HttpClient based on the proxy config and base address
        /// </summary>
        public static HttpClient GetClient(string baseAddress, IProxyConfig proxyConfig)
        {
            var httpClient = new HttpClient();

            if (proxyConfig?.EnableProxy == true && !proxyConfig.UseDefaultWebProxy)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri($"http://{proxyConfig.Address}:{proxyConfig.Port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = true
                };

                httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy });
            }

            httpClient.BaseAddress = new Uri(baseAddress);

            return httpClient;
        }
    }
}