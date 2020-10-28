// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moryx.Communication;
using Newtonsoft.Json;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base class to connect to an web http service hosted by the runtime
    /// </summary>
    public abstract class WebHttpServiceConnectorBase : IHttpServiceConnector
    {
        private readonly IVersionServiceManager _endpointService;
        private bool _isAvailable;

        /// <summary>
        /// Name of the service interface to connect to
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Gets the current client version of the client
        /// </summary>
        protected abstract string ClientVersion { get; }

        /// <summary>
        /// Underlying http client used for the communication
        /// </summary>
        protected HttpClient HttpClient { get; private set; }

        /// <inheritdoc />
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                AvailabilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Create connector using the client factories version service
        /// </summary>
        protected WebHttpServiceConnectorBase(IWcfClientFactory clientFactory)
        {
            _endpointService = ((BaseWcfClientFactory)clientFactory).VersionService;
        }

        /// <summary>
        /// Create connector with bare connection settings
        /// </summary>
        protected WebHttpServiceConnectorBase(string host, int port, IProxyConfig proxyConfig)
        {
            _endpointService = new VersionServiceManager(proxyConfig, host, port);
        }

        /// <inheritdoc />
        public void Start()
        {
            TryFetchEndpoint();
        }

        private void TryFetchEndpoint()
        {
            _endpointService.ServiceEndpointsAsync(ServiceName)
                .ContinueWith(EvaluateResponse).ConfigureAwait(false);
        }

        private async Task EvaluateResponse(Task<Endpoint[]> resp)
        {
            //Try again or dispose old client
            if (resp.Status != TaskStatus.RanToCompletion || resp.Result.Length == 0)
            {
                await CallbackAndTryFetch(ConnectionState.FailedTry);
                return;
            }

            // Parse endpoint url
            var endpoint = resp.Result.FirstOrDefault(e => e.Binding == ServiceBindingType.WebHttp);
            if (endpoint == null || string.IsNullOrEmpty(endpoint.Address))
            {
                await CallbackAndTryFetch(ConnectionState.FailedTry);
                return;
            }

            var clientVersion = Version.Parse(ClientVersion);
            var serverVersion = Version.Parse(endpoint.Version);

            // Compare version
            if (serverVersion.Major == clientVersion.Major & serverVersion >= clientVersion)
            {
                // Create new base address client
                HttpClient = new HttpClient {BaseAddress = new Uri(endpoint.Address)};

                IsAvailable = true;

                await ConnectionCallback(ConnectionState.Success);
            }
            else
            {
                await CallbackAndTryFetch(ConnectionState.VersionMissmatch);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            HttpClient?.Dispose();
        }

        /// <summary>
        /// Executes the callback and starts the fetching after a defined delay
        /// </summary>
        private async Task CallbackAndTryFetch(ConnectionState connectionState)
        {
            await ConnectionCallback(connectionState);
            await Task.Delay(1000);
            TryFetchEndpoint();
        }

        /// <summary>
        /// Action which is called if the client gets connected or failed
        /// </summary>
        public virtual Task ConnectionCallback(ConnectionState connectionState)
        {
#if HAVE_TASK_COMPLETEDTASK
            return Task.CompletedTask;
#else
            return Task.FromResult(true);
#endif
        }

        /// <summary>
        /// Get data from URL
        /// </summary>
        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await HttpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Post data to URL
        /// </summary>
        protected async Task<T> PostAsync<T>(string url, object payload)
        {
            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await HttpClient.PostAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Put new data on endpoint
        /// </summary>
        protected async Task<T> PutAsync<T>(string url, object payload)
        {
            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await HttpClient.PutAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Delete on endpoint
        /// </summary>
        protected async Task<bool> DeleteAsync(string url)
        {
            var response = await HttpClient.DeleteAsync(url);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <inheritdoc />
        public event EventHandler AvailabilityChanged;
    }
}