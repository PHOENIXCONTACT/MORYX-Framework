// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moryx.Communication;
using Newtonsoft.Json;

namespace Moryx.Tools.Wcf
{
    internal class VersionServiceManager : IVersionServiceManager
    {
        private const string ServiceName = "endpoints";

        private HttpClient _client;

        public VersionServiceManager(IProxyConfig proxyConfig, string host, int port)
        {
            // Create HttpClient
            if (proxyConfig?.EnableProxy == true && !proxyConfig.UseDefaultWebProxy)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri($"http://{proxyConfig.Address}:{proxyConfig.Port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = true
                };

                _client = new HttpClient(new HttpClientHandler { Proxy = proxy });
            }
            else
            {
                _client = new HttpClient();
            }
            _client.BaseAddress = new Uri($"http://{host}:{port}/{ServiceName}/");
        }

        ///
        public void Dispose()
        {
            _client.Dispose();
            _client = null;
        }

        public Endpoint[] ActiveEndpoints()
        {
            try
            {
                return ActiveEndpointsAsync().Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Endpoint[]> ActiveEndpointsAsync()
        {
            var response = await _client.GetStringAsync("");
            return JsonConvert.DeserializeObject<Endpoint[]>(response);
        }


        public Endpoint[] ServiceEndpoints(string service)
        {
            try
            {
                return ServiceEndpointsAsync(service).Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Endpoint[]> ServiceEndpointsAsync(string service)
        {
            var response = await _client.GetStringAsync($"service/{service}");
            return JsonConvert.DeserializeObject<Endpoint[]>(response);
        }
    }
}
