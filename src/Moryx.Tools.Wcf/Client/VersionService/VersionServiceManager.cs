// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using Moryx.Communication;
using Moryx.Configuration;
using Newtonsoft.Json;

namespace Moryx.Tools.Wcf
{
    internal class VersionServiceManager : IVersionServiceManager
    {
        private const string ServiceName = "endpoints";

        #region Fields and Properties

        public bool IsInitialized { get; private set; }

        private HttpClient _client;

        #endregion

        ///
        public void Initialize(IProxyConfig proxyConfig, string host, int port)
        {
            if (IsInitialized)
                return;

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

            IsInitialized = true;
        }

        ///
        public void Dispose()
        {
            if (!IsInitialized)
                return;

            _client.Dispose();
            _client = null;
            IsInitialized = false;
        }

        public Endpoint[] ActiveEndpoints()
        {
            try
            {
                var response = _client.GetStringAsync("").Result;
                return JsonConvert.DeserializeObject<Endpoint[]>(response);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public Endpoint[] ServiceEndpoints(string service)
        {
            try
            {
                var response = _client.GetStringAsync($"service/{service}").Result;
                return JsonConvert.DeserializeObject<Endpoint[]>(response);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
